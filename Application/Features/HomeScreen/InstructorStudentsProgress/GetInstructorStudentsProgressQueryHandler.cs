using Application.DTOs.HomeScreen;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.HomeScreen.InstructorStudentsProgress
{
    public class GetInstructorStudentsProgressQueryHandler(
        IUnitOfWork unitOfWork,
        IInstructorContentScopeService instructorContentScopeService)
        : IRequestHandler<GetInstructorStudentsProgressQuery, Result<InstructorStudentsProgressResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IInstructorContentScopeService _instructorContentScopeService = instructorContentScopeService;

        public async Task<Result<InstructorStudentsProgressResponse>> Handle(
            GetInstructorStudentsProgressQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var scope = await _instructorContentScopeService.ResolveAsync(
                    request.InstructorId,
                    request.CourseId,
                    request.SectionId,
                    cancellationToken);

                var repo = _unitOfWork.GetRepository<IEnrollmentProgressRepository>();
                var response = await repo.GetInstructorStudentsProgressAsync(
                    request.InstructorId,
                    scope.CourseIds,
                    scope.SectionIds,
                    request.StudentId,
                    request.Page,
                    request.PageSize,
                    cancellationToken);

                return Result<InstructorStudentsProgressResponse>.Success(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<InstructorStudentsProgressResponse>.FailureStatusCode(
                    ex.Message,
                    ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<InstructorStudentsProgressResponse>.FailureStatusCode(
                    $"An error occurred while retrieving student progress: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
