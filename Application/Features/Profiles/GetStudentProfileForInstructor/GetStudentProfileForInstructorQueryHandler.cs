using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Centers.Interfaces;
using Application.Features.HomeScreen.Interfaces;
using Application.Features.Profiles.DTOs;
using Application.Features.Profiles.Interfaces;
using Domain.enums;
using MediatR;

namespace Application.Features.Profiles.GetStudentProfileForInstructor
{
    public class GetStudentProfileForInstructorQueryHandler(
        IUnitOfWork unitOfWork,
        IInstructorContentScopeService instructorContentScopeService)
        : IRequestHandler<GetStudentProfileForInstructorQuery, Result<StudentProfileForInstructorResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IInstructorContentScopeService _instructorContentScopeService = instructorContentScopeService;

        public async Task<Result<StudentProfileForInstructorResponse>> Handle(
            GetStudentProfileForInstructorQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IProfileRepository>();

                if (!await repo.StudentExistsAsync(request.StudentId, cancellationToken))
                {
                    return Result<StudentProfileForInstructorResponse>.FailureStatusCode(
                        "Student not found.",
                        ErrorType.NotFound);
                }

                if (!await repo.HasSharedContentAsync(
                        request.InstructorId,
                        request.StudentId,
                        cancellationToken))
                {
                    return Result<StudentProfileForInstructorResponse>.FailureStatusCode(
                        "You are not eligible to view this student profile.",
                        ErrorType.Forbidden);
                }

                var profile = await repo.GetStudentProfileForInstructorAsync(
                    request.InstructorId,
                    request.StudentId,
                    cancellationToken);

                if (profile is null)
                {
                    return Result<StudentProfileForInstructorResponse>.FailureStatusCode(
                        "Student not found.",
                        ErrorType.NotFound);
                }

                var scope = await _instructorContentScopeService.ResolveAsync(
                    request.InstructorId,
                    courseId: null,
                    sectionId: null,
                    cancellationToken);

                var progressRepo = _unitOfWork.GetRepository<IEnrollmentProgressRepository>();
                var progress = await progressRepo.GetInstructorStudentsProgressAsync(
                    request.InstructorId,
                    scope.CourseIds,
                    scope.SectionIds,
                    request.StudentId,
                    page: 1,
                    pageSize: 1,
                    cancellationToken);

                profile.Enrollments = progress.Students.Items.FirstOrDefault()?.Enrollments ?? [];

                return Result<StudentProfileForInstructorResponse>.Success(profile);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<StudentProfileForInstructorResponse>.FailureStatusCode(
                    ex.Message,
                    ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<StudentProfileForInstructorResponse>.FailureStatusCode(
                    $"An error occurred while retrieving the student profile: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
