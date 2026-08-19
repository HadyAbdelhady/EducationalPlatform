using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Centers.Interfaces;
using Application.Features.HomeScreen.DTOs;
using Application.Features.HomeScreen.Interfaces;
using Application.Features.Profiles.Interfaces;
using Domain.enums;
using MediatR;

namespace Application.Features.HomeScreen.InstructorStudentEnrollments
{
    public class GetInstructorStudentEnrollmentsQueryHandler(
        IUnitOfWork unitOfWork,
        IInstructorContentScopeService instructorContentScopeService)
        : IRequestHandler<GetInstructorStudentEnrollmentsQuery, Result<InstructorStudentEnrollmentsResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IInstructorContentScopeService _instructorContentScopeService = instructorContentScopeService;

        public async Task<Result<InstructorStudentEnrollmentsResponse>> Handle(
            GetInstructorStudentEnrollmentsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var profileRepo = _unitOfWork.GetRepository<IProfileRepository>();
                if (!await profileRepo.StudentExistsAsync(request.StudentId, cancellationToken))
                {
                    return Result<InstructorStudentEnrollmentsResponse>.FailureStatusCode(
                        "Student not found.",
                        ErrorType.NotFound);
                }

                if (!await profileRepo.HasSharedContentAsync(
                        request.InstructorId,
                        request.StudentId,
                        cancellationToken))
                {
                    return Result<InstructorStudentEnrollmentsResponse>.FailureStatusCode(
                        "You are not eligible to view this student.",
                        ErrorType.Forbidden);
                }

                var scope = await _instructorContentScopeService.ResolveAsync(
                    request.InstructorId,
                    courseId: null,
                    sectionId: null,
                    cancellationToken);

                var progressRepo = _unitOfWork.GetRepository<IEnrollmentProgressRepository>();
                var response = await progressRepo.GetInstructorStudentEnrollmentsAsync(
                    request.InstructorId,
                    request.StudentId,
                    scope.CourseIds,
                    scope.SectionIds,
                    cancellationToken);

                return Result<InstructorStudentEnrollmentsResponse>.Success(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<InstructorStudentEnrollmentsResponse>.FailureStatusCode(
                    ex.Message,
                    ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<InstructorStudentEnrollmentsResponse>.FailureStatusCode(
                    $"An error occurred while retrieving student enrollments: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
