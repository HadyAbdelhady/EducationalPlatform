using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Students.DTOs;
using Application.Features.Students.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Students.Commands.IncrementScreenshotTrial
{
    public class IncrementScreenshotTrialCommandHandler(
        IStudentRepository studentRepository,
        ICloudinaryCore cloudinaryService)
        : IRequestHandler<IncrementScreenshotTrialCommand, Result<StudentScreenshotStatusDto>>
    {
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<StudentScreenshotStatusDto>> Handle(
            IncrementScreenshotTrialCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request.ImageFile is null || request.ImageFile.Length == 0)
                {
                    return Result<StudentScreenshotStatusDto>.FailureStatusCode(
                        "A screenshot image file is required.",
                        ErrorType.BadRequest);
                }

                var student = await _studentRepository.GetStudentByIdAsync(request.StudentId, cancellationToken);
                if (student is null)
                {
                    return Result<StudentScreenshotStatusDto>.FailureStatusCode("Student not found", ErrorType.NotFound);
                }

                var imageUrl = await _cloudinaryService.UploadMediaAsync(
                    request.ImageFile,
                    UsageCategory.Screenshot);

                var screenshot = new StudentScreenshot
                {
                    Id = Guid.NewGuid(),
                    StudentId = student.UserId,
                    ImageUrl = imageUrl,
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
                    ScreenshotUrl = imageUrl,
                    AttemptedAt = screenshot.CreatedAt,
                    Message = "Screenshot attempt recorded successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return Result<StudentScreenshotStatusDto>.FailureStatusCode(ex.Message, ErrorType.BadRequest);
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
