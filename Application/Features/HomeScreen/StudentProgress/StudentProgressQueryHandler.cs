using Application.DTOs.HomeScreen;
using Application.HelperFunctions;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.HomeScreen.StudentProgress
{
    public class StudentProgressQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<StudentProgressQuery, Result<StudentProgressSummaryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<StudentProgressSummaryResponse>> Handle(
            StudentProgressQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IHomeScreenRepository>();
                var response = await repo.GetStudentProgressDataAsync(
                    request.StudentId,
                    request.CoursesPage,
                    request.CoursesPageSize,
                    request.MilestonesPage,
                    request.MilestonesPageSize,
                    cancellationToken);

                response.AverageGradeLetter = GradeMapping.ToLetterGrade(response.AverageGrade);
                return Result<StudentProgressSummaryResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<StudentProgressSummaryResponse>.FailureStatusCode(
                    $"An error occurred while retrieving student progress: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
