using System.Security.Claims;
using Application.Features.Profiles.GetInstructorProfileForStudent;
using Application.Features.Profiles.GetStudentProfileForInstructor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Features.Profiles
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController(IMediator mediator, ILogger<ProfileController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<ProfileController> _logger = logger;

        [HttpGet("students/{studentId:guid}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetStudentProfile(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            var instructorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorIdClaim) || !Guid.TryParse(instructorIdClaim, out var instructorId))
                return Unauthorized();

            _logger.LogInformation(
                "Instructor {InstructorId} requesting student profile {StudentId}",
                instructorId,
                studentId);

            var result = await _mediator.Send(
                new GetStudentProfileForInstructorQuery
                {
                    InstructorId = instructorId,
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
    }
}
