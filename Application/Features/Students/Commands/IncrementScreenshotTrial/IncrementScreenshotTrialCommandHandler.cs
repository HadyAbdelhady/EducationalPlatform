using Application.Common;
using Application.Features.Students.DTOs;
using Application.Features.Students.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Students.Commands.IncrementScreenshotTrial
{
    public class IncrementScreenshotTrialCommandHandler(IStudentRepository studentRepository)
        : IRequestHandler<IncrementScreenshotTrialCommand, Result<StudentScreenshotStatusDto>>
    {
        private readonly IStudentRepository _studentRepository = studentRepository;

        public async Task<Result<StudentScreenshotStatusDto>> Handle(
            IncrementScreenshotTrialCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var student = await _studentRepository.GetStudentByIdAsync(request.StudentId, cancellationToken);
                if (student is null)
                {
                    return Result<StudentScreenshotStatusDto>.FailureStatusCode("Student not found", ErrorType.NotFound);
                }

                var screenshot = new StudentScreenshot
                {
                    Id = Guid.NewGuid(),
                    StudentId = student.UserId,
                    EntityType = request.EntityType,
                    EntityId = request.EntityId,
                    PageName = request.PageName,
                    CreatedAt = EgyptTime.UtcNow
                };

                await _studentRepository.AddScreenshotAsync(screenshot, cancellationToken);

                student.ScreenshotTrials += 1;
                student.TriedScreenshot = true;

                await _studentRepository.UpdateAsync(student, cancellationToken);

                return Result<StudentScreenshotStatusDto>.Success(new StudentScreenshotStatusDto
                {
                    StudentId = student.UserId,
                    ScreenshotTrials = student.ScreenshotTrials,
                    TriedScreenshot = student.TriedScreenshot,
                    EntityType = screenshot.EntityType,
                    EntityId = screenshot.EntityId,
                    PageName = screenshot.PageName,
                    AttemptedAt = screenshot.CreatedAt,
                    Message = "Screenshot attempt recorded successfully."
                });
            }
            catch (Exception ex)
            {
                return Result<StudentScreenshotStatusDto>.FailureStatusCode(
                    $"An error occurred while recording the screenshot attempt: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
