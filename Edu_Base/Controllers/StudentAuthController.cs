using Application.DTOs.Auth;
using Application.Features.Auth.Commands.GoogleLogout;
using Application.Features.Auth.Commands.StudentGoogleLogin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentAuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StudentAuthController> _logger;

        public StudentAuthController(IMediator mediator, ILogger<StudentAuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("google-login")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GoogleLogin(
            [FromBody] StudentGoogleLoginRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Student Google login attempt for device: {DeviceId}", request.DeviceId);

                var command = new StudentGoogleLoginCommand
                {
                    IdToken = request.IdToken,
                    DeviceId = request.DeviceId,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    EducationYear = request.EducationYear,
                    LocationMaps = request.LocationMaps
                };

                var result = await _mediator.Send(command, cancellationToken);

                _logger.LogInformation(
                    "Student Google login successful. UserId: {UserId}, IsNewUser: {IsNewUser}",
                    result.UserId,
                    result.IsNewUser);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized student Google login attempt");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during student Google login");
                return StatusCode(500, new { message = "An error occurred during authentication" });
            }
        }

        [HttpPost("logout")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout(
            [FromBody] Guid userId,
            CancellationToken cancellationToken)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                _logger.LogInformation("Student logout attempt for UserId: {UserId}", userId);

                var command = new GoogleLogoutCommand { UserId = userId };
                var result = await _mediator.Send(command, cancellationToken);

                _logger.LogInformation("Student logout successful for UserId: {UserId}", userId);

                return Ok(new { success = result, message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during student logout for UserId: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }
    }
}
