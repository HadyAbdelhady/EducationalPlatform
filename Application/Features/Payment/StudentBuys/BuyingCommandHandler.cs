using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Payment.StudentBuys
{
    public class BuyingCommandHandler(
        IUnitOfWork unitOfWork,
        IStudentEducationYearProvider studentEducationYearProvider) : IRequestHandler<BuyingCommand, Result<StudentBuyResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IStudentEducationYearProvider _studentEducationYearProvider = studentEducationYearProvider;

        public async Task<Result<StudentBuyResponse>> Handle(BuyingCommand request, CancellationToken cancellationToken)
        {
            var enrollmentRepo = _unitOfWork.GetRepository<IStudentEnrollmentRepository>();
            var studentEducationYearId = await _studentEducationYearProvider
                .GetEducationYearIdByUserIdAsync(request.StudentId, cancellationToken);

            if (!studentEducationYearId.HasValue)
            {
                return Result<StudentBuyResponse>.FailureStatusCode(
                    "Student not found or has no education year assigned.",
                    ErrorType.BadRequest);
            }

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

                if (!existingEnrollment)
                {
                    await enrollmentRepo.AddStudentCourseAsync(new StudentCourse
                    {
                        StudentId = request.StudentId,
                        CourseId = request.EntityId
                    }, cancellationToken);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
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

                if (!existingEnrollment)
                {
                    await enrollmentRepo.AddStudentSectionAsync(new StudentSection
                    {
                        StudentId = request.StudentId,
                        SectionId = request.EntityId,
                        Progress = 0
                    }, cancellationToken);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            var response = new StudentBuyResponse
            {
                StudentId = request.StudentId,
                EntityId = request.EntityId,
                EntityToBuy = request.EntityToBuy
            };
            return Result<StudentBuyResponse>.Success(response);
        }
    }
}
