using Application.Common;
using Application.Features.Students.DTOs;
using Application.Features.Students.Interfaces;
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
            var student = await _studentRepository.GetStudentByIdAsync(request.StudentId, cancellationToken);
            if (student is null)
            {
                return Result<StudentScreenshotStatusDto>.FailureStatusCode("Student not found", ErrorType.NotFound);
            }

            student.ScreenshotTrials += 1;
            student.TriedScreenshot = true;

            await _studentRepository.UpdateAsync(student, cancellationToken);

            return Result<StudentScreenshotStatusDto>.Success(new StudentScreenshotStatusDto
            {
                StudentId = student.UserId,
                ScreenshotTrials = student.ScreenshotTrials,
                TriedScreenshot = student.TriedScreenshot,
                Message = "Screenshot attempt recorded successfully."
            });
        }
    }
}
