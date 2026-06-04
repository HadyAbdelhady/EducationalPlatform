using Application.DTOs.HomeScreen;
using Application.HelperFunctions;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.HomeScreen.StudentProgress
{
    public class StudentProgressQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<StudentProgressQuery, Result<StudentEnrollmentProgressResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<StudentEnrollmentProgressResponse>> Handle(
            StudentProgressQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IEnrollmentProgressRepository>();
                var response = await repo.GetStudentEnrollmentProgressAsync(
                    request.StudentId,
                    request.EnrollmentsPage,
                    request.EnrollmentsPageSize,
                    request.MilestonesPage,
                    request.MilestonesPageSize,
                    cancellationToken);

                response.Global.AverageGradeLetter = GradeMapping.ToLetterGrade(response.Global.AverageGrade);
                return Result<StudentEnrollmentProgressResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<StudentEnrollmentProgressResponse>.FailureStatusCode(
                    $"An error occurred while retrieving student progress: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
