using Application.DTOs.Auth;
using Application.Features.Auth.Commands.GoogleLogout;
using Application.Features.Auth.Commands.StudentGoogleLogin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentAuthController(IMediator mediator, ILogger<StudentAuthController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<StudentAuthController> _logger = logger;

        /// <summary>
        /// Google login endpoint. Now consolidated with login - creates account if new, returns existing if already registered.
        /// </summary>
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(
            [FromBody] StudentGoogleLoginRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Student Google signup/login attempt for device: {DeviceId}", request.DeviceId);

                // Use the consolidated login command which handles both signup and login
                var command = new StudentGoogleLoginCommand
                {
                    IdToken = request.IdToken,
                    DeviceId = request.DeviceId,
                    Ssn = request.Ssn,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    EducationYear = request.EducationYear,
                    LocationMaps = request.LocationMaps
                };

                var result = await _mediator.Send(command, cancellationToken);

                _logger.LogInformation(
                    "Student Google authentication successful. UserId: {UserId}, IsNewUser: {IsNewUser}",
                    result.UserId,
                    result.IsNewUser);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized student Google authentication attempt");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during student Google authentication");
                return StatusCode(500, new { message = "An error occurred during authentication" });
            }
        }

        [HttpPost("logout")]
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
