using Application.Features.Payment.DTOs;
using Application.Features.Payment.DTOs.PaymobRawDtos;
using Application.Common.Interfaces;
using Application.Features.EducationYears.Interfaces;
using Application.Features.HomeScreen.Interfaces;
using Application.Features.Payment.Interfaces;
using Application.Common;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Payment.CreatePaymentIntension
{
    public class BuyingCommandHandler(
        IPaymentService paymentService,
        IUnitOfWork unitOfWork,
        IStudentEducationYearProvider studentEducationYearProvider) : IRequestHandler<BuyingCommand, Result<StudentBuyResponse>>
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IStudentEducationYearProvider _studentEducationYearProvider = studentEducationYearProvider;

        public async Task<Result<StudentBuyResponse>> Handle(BuyingCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.GetRepository<Auth.Interfaces.IUserRepository>()
                .GetByIdAsync(request.StudentId, cancellationToken);

            if (user == null)
            {
                return Result<StudentBuyResponse>.FailureStatusCode("Student user not found.", ErrorType.NotFound);
            }

            var nameParts = user.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var firstName = nameParts.Length > 0 ? nameParts[0] : "Student";
            var lastName = nameParts.Length > 1 ? string.Join(' ', nameParts.Skip(1)) : "User";
            var email = !string.IsNullOrWhiteSpace(user.GmailExternal) ? user.GmailExternal : "student@educationalplatform.com";

            var studentInfo = new DTOs.Student(firstName, lastName, email);

            var enrollmentRepo = _unitOfWork.GetRepository<IStudentEnrollmentRepository>();
            var studentEducationYearId = await _studentEducationYearProvider
                .GetEducationYearIdByUserIdAsync(request.StudentId, cancellationToken);

            if (!studentEducationYearId.HasValue)
            {
                return Result<StudentBuyResponse>.FailureStatusCode(
                    "Student not found or has no education year assigned.",
                    ErrorType.BadRequest);
            }

            var testingPay = new PaymentInitiationRequest
            {
                EntityId = request.EntityId,
                EntityType = request.EntityToBuy,
                PaymentMethods = request.PaymentMethods,
                Student = studentInfo,
            };

            var payment = new PaymentTransactions
            {
                Id = Guid.NewGuid(),
                StudentId = request.StudentId,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow
            };

            // add the payment transaction to the database before processing the payment - IMPORTANT: this ensures that we have a record of the transaction even if the payment processing fails
            await _unitOfWork.Repository<PaymentTransactions>().AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            string clientSecret = string.Empty;

            if (request.EntityToBuy == EntityToBuy.Course)
            {
                var course = await _unitOfWork.Repository<Course>().GetByIdAsync(request.EntityId, cancellationToken);
                if (course == null)
                {
                    return Result<StudentBuyResponse>.FailureStatusCode("Course not found.", ErrorType.NotFound);
                }

                if (course.EducationYearId != studentEducationYearId.Value)
                {
                    return Result<StudentBuyResponse>.FailureStatusCode(
                        "Cannot enroll in a course from a different education year.",
                        ErrorType.BadRequest);
                }

                var existingEnrollment = await enrollmentRepo.IsStudentEnrolledInCourseAsync(
                    request.StudentId,
                    request.EntityId,
                    cancellationToken);

                if (existingEnrollment)
                    return Result<StudentBuyResponse>.FailureStatusCode("Student is already enrolled.", ErrorType.Conflict);


                testingPay.Money = new((decimal)course.Price!, "EGP");
                testingPay.Items = [new OrderItem
                    {
                        Name = course.Name,
                        Amount = (int)course.Price,
                        Description = course.Description ?? "N/A",
                        Quantity = 1
                    }];

                var Intention = await _paymentService.CreateIntentionAsync(
                    testingPay,
                    cancellationToken);

              if (string.IsNullOrEmpty(Intention.Id))
                {
                    payment.Status = PaymentStatus.Failed;
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<StudentBuyResponse>.FailureStatusCode("Failed to initiate payment with the provider.", ErrorType.InternalServerError);
                }

                payment.Amount = (decimal)course.Price!;
                payment.CourseId = course.Id;
                payment.PaymobIntentionId = Intention.Id;
                clientSecret = Intention.ClientSecret;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

            }
            else
            {
                var section = await _unitOfWork.Repository<Section>()
                    .GetByIdAsync(request.EntityId, cancellationToken, s => s.Course!);

                if (section == null)
                {
                    return Result<StudentBuyResponse>.FailureStatusCode("Section not found.", ErrorType.NotFound);
                }

                if (section.Course == null || section.Course.EducationYearId != studentEducationYearId.Value)
                {
                    return Result<StudentBuyResponse>.FailureStatusCode(
                        "Cannot enroll in a section from a different education year.",
                        ErrorType.BadRequest);
                }

                var existingEnrollment = await enrollmentRepo.IsStudentEnrolledInSectionAsync(
                    request.StudentId,
                    request.EntityId,
                    cancellationToken);

                if (existingEnrollment)
                    return Result<StudentBuyResponse>.FailureStatusCode("Student is already enrolled.", ErrorType.Conflict);

                testingPay.Money = new(section.Price!, "EGP");
                testingPay.Items = [new OrderItem
                    {
                        Name = section.Name,
                        Amount = (int)section.Price,
                        Description = section.Description ?? "N/A",
                        Quantity = 1
                    }];

                var Intention = await _paymentService.CreateIntentionAsync(
                   testingPay,
                   cancellationToken);

                // Paymob returns confirmed=false on a freshly created intention (not yet paid).
                // A real failure means no Id was returned.
                if (string.IsNullOrEmpty(Intention.Id))
                {
                    payment.Status = PaymentStatus.Failed;
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<StudentBuyResponse>.FailureStatusCode("Failed to initiate payment with the provider.", ErrorType.InternalServerError);
                }

                payment.Amount = section!.Price!;
                payment.SectionId = section.Id;
                payment.PaymobIntentionId = Intention.Id;
                clientSecret = Intention.ClientSecret;

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }


            var response = new StudentBuyResponse
            {
                StudentId = request.StudentId,
                EntityId = request.EntityId,
                EntityToBuy = request.EntityToBuy,
                PamobData = new PaymentData
                {
                    ClientSecret = clientSecret,
                    PaymentId = payment.PaymobIntentionId!,
                }
            };
            return Result<StudentBuyResponse>.Success(response);
        }
    }
}
