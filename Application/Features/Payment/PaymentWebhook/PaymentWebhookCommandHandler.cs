using Application.DTOs.Payment.PaymobRawDtos;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Payment.PaymentWebhook
{
    public class PaymentWebhookCommandHandler(IPaymentService paymentService, IUnitOfWork unitOfWork) : IRequestHandler<PaymentWebhookCommand, Result<bool>>
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<bool>> Handle(PaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            var payload = request.Payload.Obj;

            string concatenatedString = ConcatenatePayload(payload);

            var isValidHmac = _paymentService.VerifyHmacSignature(concatenatedString, request.HmacSignature);
            if (!isValidHmac)
                return Result<bool>.FailureStatusCode("Invalid HMAC signature", ErrorType.UnAuthorized);

            var isSuccess = request.Payload.Obj.Success;
            var intentionId = request.Payload.Obj.Order?.Id;

            if (!intentionId.HasValue)
                return Result<bool>.FailureStatusCode("Intention ID is missing", ErrorType.BadRequest);

            var intentionIdString = intentionId.Value.ToString();
            var PaymentTransaction = await _unitOfWork.Repository<PaymentTransactions>()
                   .FirstOrDefaultAsync(x => x.PaymobIntentionId == intentionIdString, cancellationToken);

            if (PaymentTransaction == null)
                return Result<bool>.FailureStatusCode("Payment transaction not found", ErrorType.NotFound);

            if (!isSuccess)
            {
                PaymentTransaction.Status = PaymentStatus.Failed;
                PaymentTransaction.UpdatedAt = EgyptTime.UtcNow;

                _unitOfWork.Repository<PaymentTransactions>().Update(PaymentTransaction);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<bool>.Success(true);

            }

            // IDENPOTENCY : If the payment transaction is already completed, we don't need to process it again.
            if (PaymentTransaction.Status == PaymentStatus.Completed)
                // Acknowledge the webhook successfully since we already processed it
                return Result<bool>.Success(true);
            


            PaymentTransaction.Status = PaymentStatus.Completed;
            PaymentTransaction.UpdatedAt = EgyptTime.UtcNow;

            var User = await _unitOfWork.GetRepository<IUserRepository>().GetByIdAsync(PaymentTransaction.StudentId, cancellationToken);

            if (User == null)
                return Result<bool>.FailureStatusCode("User not found", ErrorType.NotFound);

            var student = User.Student;
            if (student == null)
                return Result<bool>.FailureStatusCode("Student not found", ErrorType.NotFound);


            if (PaymentTransaction.CourseId.HasValue)
            {
                var StudentEnrollment = new StudentCourse
                {
                    StudentId = student.UserId,
                    CourseId = PaymentTransaction.CourseId.Value,
                };
                student.StudentCourses.Add(StudentEnrollment);
            }
            else
            {
                var studentEnrollment = new StudentSection()
                {
                    StudentId = student.UserId,
                    SectionId = PaymentTransaction.SectionId!.Value,

                };
                student.StudentSections.Add(studentEnrollment);
            }
            _unitOfWork.Repository<PaymentTransactions>().Update(PaymentTransaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);

        }
        private string ConcatenatePayload(PaymobWebhookObj payload)
        {

            // 1. Paymob requires strict lexicographical concatenation of these specific fields:
            return
                $"{payload.AmountCents}" +
                $"{payload.CreatedAt}" +
                $"{payload.Currency}" +
                $"{payload.ErrorOccured.ToString().ToLower()}" +
                $"{payload.HasParentTransaction.ToString().ToLower()}" +
                $"{payload.Id}" +
                $"{payload.IntegrationId}" +
                $"{payload.Is3DSecure.ToString().ToLower()}" +
                $"{payload.IsAuth.ToString().ToLower()}" +
                $"{payload.IsCapture.ToString().ToLower()}" +
                $"{payload.IsRefunded.ToString().ToLower()}" +
                $"{payload.IsStandalonePayment.ToString().ToLower()}" +
                $"{payload.IsVoided.ToString().ToLower()}" +
                $"{payload.Order?.Id}" +
                $"{payload.Owner}" +
                $"{payload.Pending.ToString().ToLower()}" +
                $"{payload.SourceData?.Pan}" +
                $"{payload.SourceData?.SubType}" +
                $"{payload.SourceData?.Type}" +
                $"{payload.Success.ToString().ToLower()}";
        }
    }
}
