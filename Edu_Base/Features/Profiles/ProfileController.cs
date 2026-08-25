using System.Security.Claims;
using Application.Common.Interfaces;
using Application.Features.Profiles.Commands.UpdateStudentProfilePicture;
using Application.Features.Profiles.GetInstructorProfileForStudent;
using Application.Features.Profiles.GetStudentProfileForInstructor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Features.Profiles
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController(
        IMediator mediator,
        ILogger<ProfileController> logger,
        ICurrentUserService currentUser) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<ProfileController> _logger = logger;
        private readonly ICurrentUserService _currentUser = currentUser;

        [HttpGet("students/{studentId:guid}")]
        [Authorize(Roles = "Instructor,Student")]
        public async Task<IActionResult> GetStudentProfile(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUser.TryGetUserId(out var requesterId))
                return Unauthorized();

            _logger.LogInformation(
                "User {RequesterId} requesting student profile {StudentId}",
                requesterId,
                studentId);

            var result = await _mediator.Send(
                new GetStudentProfileForInstructorQuery
                {
                    InstructorId = requesterId,
                    StudentId = studentId
                },
                cancellationToken);

            return result.IsSuccess
                ? Ok(result)
                : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("instructors/{instructorId:guid}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetInstructorProfile(
            Guid instructorId,
            CancellationToken cancellationToken = default)
        {
            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentIdClaim) || !Guid.TryParse(studentIdClaim, out var studentId))
                return Unauthorized();

            _logger.LogInformation(
                "Student {StudentId} requesting instructor profile {InstructorId}",
                studentId,
                instructorId);

            var result = await _mediator.Send(
                new GetInstructorProfileForStudentQuery
                {
                    StudentId = studentId,
                    InstructorId = instructorId
                },
                cancellationToken);

            return result.IsSuccess
                ? Ok(result)
                : StatusCode((int)result.ErrorType, result);
        }

        /// <summary>
        /// Upload or replace the authenticated student's profile picture.
        /// Use from the profile page, or from the login/onboarding screen after Google login.
        /// PATCH /api/Profile/picture
        /// Content-Type: multipart/form-data, field name: file
        /// </summary>
        [HttpPatch("picture")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateProfilePicture(
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUser.TryGetUserId(out var studentId))
                return Unauthorized("User id not found in token.");

            if (file is null || file.Length == 0)
                return BadRequest("A profile picture file is required.");

            var result = await _mediator.Send(
                new UpdateStudentProfilePictureCommand
                {
                    StudentId = studentId,
                    PictureFile = file
                },
                cancellationToken);

            return result.IsSuccess
                ? Ok(result)
                : StatusCode((int)result.ErrorType, result);
        }
    }
}
