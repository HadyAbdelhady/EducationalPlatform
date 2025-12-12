using Application.DTOs.Auth;
using Application.Features.Auth.Commands.GoogleLogout;
using Application.Features.Auth.Commands.InstructorGoogleLogin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstructorAuthController(IMediator mediator, ILogger<InstructorAuthController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<InstructorAuthController> _logger = logger;

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(
            [FromBody] InstructorGoogleLoginRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Instructor Google login attempt");

            var command = new InstructorGoogleLoginCommand
            {
                IdToken = request.IdToken,
                Ssn = request.Ssn,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                //EducationYear = request.EducationYear,
                LocationMaps = request.LocationMaps
            };

            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation(
                "Instructor Google login successful. UserId: {UserId}, IsNewUser: {IsNewUser}",
                result.Value.UserId,
                result.Value.IsNewUser);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);

        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
            [FromBody] Guid userId,
            CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }
            _logger.LogInformation("Instructor logout attempt for UserId: {UserId}", userId);

            var command = new GoogleLogoutCommand { UserId = userId };
            var result = await _mediator.Send(command, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);

        }
    }
}
