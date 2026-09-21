using Application.Common.Interfaces;
using Application.Features.Students.Commands.IncrementScreenshotTrial;
using Application.Features.Students.Commands.ResetScreenshotTrial;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Features.Students
{
    [ApiController]
    [Route("api/students")]
    public class StudentController(
        IMediator mediator,
        ILogger<StudentController> logger,
        ICurrentUserService currentUser) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<StudentController> _logger = logger;
        private readonly ICurrentUserService _currentUser = currentUser;

        /// <summary>
        /// Increments the screenshot attempt counter for a student and flags them as restricted.
        /// Can be reported by the student themselves or by an instructor/admin.
        /// POST /api/students/{studentId}/screenshot-trials/increment
        /// </summary>
        [HttpPost("{studentId:guid}/screenshot-trials/increment")]
        [HttpPost("{studentId:guid}/screenshot-trial")]
        [Authorize(Roles = "Student,Instructor,CenterAdmin,Admin")]
        public async Task<IActionResult> IncrementScreenshotTrial(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            if (User.IsInRole("Student"))
            {
                if (!_currentUser.TryGetUserId(out var requesterId) || requesterId != studentId)
                {
                    return Forbid();
                }
            }

            _logger.LogInformation("Incrementing screenshot trial for student: {StudentId}", studentId);

            var command = new IncrementScreenshotTrialCommand { StudentId = studentId };
            var result = await _mediator.Send(command, cancellationToken);

            return result.IsSuccess
                ? Ok(result)
                : StatusCode((int)result.ErrorType, result);
        }

        /// <summary>
        /// Resets the screenshot restriction for a student (unblocks them by setting TriedScreenshot = false)
        /// while keeping their cumulative screenshot trials count intact.
        /// Allowed only for Instructors, Center Admins, and Admins.
        /// POST /api/students/{studentId}/screenshot-restriction/reset
        /// </summary>
        [HttpPost("{studentId:guid}/screenshot-restriction/reset")]
        [HttpPost("{studentId:guid}/screenshot-trials/reset")]
        [HttpPost("{studentId:guid}/reset-screenshot-trial")]
        [Authorize(Roles = "Instructor,CenterAdmin,Admin")]
        public async Task<IActionResult> ResetScreenshotRestriction(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Resetting screenshot restriction for student: {StudentId}", studentId);

            var command = new ResetScreenshotRestrictionCommand { StudentId = studentId };
            var result = await _mediator.Send(command, cancellationToken);

            return result.IsSuccess
                ? Ok(result)
                : StatusCode((int)result.ErrorType, result);
        }
    }
}
