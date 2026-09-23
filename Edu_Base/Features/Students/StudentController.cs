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
        /// Records an attempted screenshot by the authenticated student, uploads the captured page image to Cloudinary,
        /// creates a historical record of the attempt, increments their counter, and restricts their account.
        /// POST /api/students/screenshot-trial
        /// Content-Type: multipart/form-data
        /// </summary>
        [HttpPost("screenshot-trial")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> IncrementScreenshotTrial(
            IFormFile image,
            [FromForm] string? pageName = null,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUser.TryGetUserId(out var studentId))
            {
                return Unauthorized("User id not found in token.");
            }

            if (image is null || image.Length == 0)
            {
                return BadRequest("A screenshot image file is required.");
            }

            _logger.LogInformation("Recording screenshot trial for student: {StudentId} on page: {PageName}", studentId, pageName);

            var command = new IncrementScreenshotTrialCommand
            {
                StudentId = studentId,
                ImageFile = image,
                PageName = pageName
            };
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
