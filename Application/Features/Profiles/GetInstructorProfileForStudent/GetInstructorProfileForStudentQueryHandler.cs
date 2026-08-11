using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Profiles.DTOs;
using Application.Features.Profiles.Interfaces;
using Domain.enums;
using MediatR;

namespace Application.Features.Profiles.GetInstructorProfileForStudent
{
    public class GetInstructorProfileForStudentQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetInstructorProfileForStudentQuery, Result<InstructorProfileForStudentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<InstructorProfileForStudentResponse>> Handle(
            GetInstructorProfileForStudentQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IProfileRepository>();

                if (!await repo.InstructorExistsAsync(request.InstructorId, cancellationToken))
                {
                    return Result<InstructorProfileForStudentResponse>.FailureStatusCode(
                        "Instructor not found.",
                        ErrorType.NotFound);
                }

                if (!await repo.HasSharedContentAsync(
                        request.InstructorId,
                        request.StudentId,
                        cancellationToken))
                {
                    return Result<InstructorProfileForStudentResponse>.FailureStatusCode(
                        "You are not eligible to view this instructor profile.",
                        ErrorType.Forbidden);
                }

                var profile = await repo.GetInstructorProfileForStudentAsync(
                    request.StudentId,
                    request.InstructorId,
                    cancellationToken);

                if (profile is null)
                {
                    return Result<InstructorProfileForStudentResponse>.FailureStatusCode(
                        "Instructor not found.",
                        ErrorType.NotFound);
                }

                return Result<InstructorProfileForStudentResponse>.Success(profile);
            }
            catch (Exception ex)
            {
                return Result<InstructorProfileForStudentResponse>.FailureStatusCode(
                    $"An error occurred while retrieving the instructor profile: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
