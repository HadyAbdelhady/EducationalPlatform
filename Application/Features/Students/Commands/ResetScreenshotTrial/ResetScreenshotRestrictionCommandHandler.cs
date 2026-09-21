using Application.Common;
using Application.Features.Students.DTOs;
using Application.Features.Students.Interfaces;
using Domain.enums;
using MediatR;

namespace Application.Features.Students.Commands.ResetScreenshotTrial
{
    public class ResetScreenshotRestrictionCommandHandler(IStudentRepository studentRepository)
        : IRequestHandler<ResetScreenshotRestrictionCommand, Result<StudentScreenshotStatusDto>>
    {
        private readonly IStudentRepository _studentRepository = studentRepository;

        public async Task<Result<StudentScreenshotStatusDto>> Handle(
            ResetScreenshotRestrictionCommand request,
            CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetStudentByIdAsync(request.StudentId, cancellationToken);
            if (student is null)
            {
                return Result<StudentScreenshotStatusDto>.FailureStatusCode("Student not found", ErrorType.NotFound);
            }

            // Unblock student restriction while keeping the cumulative screenshot trials counter intact
            student.TriedScreenshot = false;

            await _studentRepository.UpdateAsync(student, cancellationToken);

            return Result<StudentScreenshotStatusDto>.Success(new StudentScreenshotStatusDto
            {
                StudentId = student.UserId,
                ScreenshotTrials = student.ScreenshotTrials,
                TriedScreenshot = student.TriedScreenshot,
                Message = "Screenshot restriction reset successfully."
            });
        }
    }
}
