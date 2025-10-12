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
            try
            {
                _logger.LogInformation("Instructor Google login attempt");

                var command = new InstructorGoogleLoginCommand
                {
                    IdToken = request.IdToken,
                    Ssn = request.Ssn,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    EducationYear = request.EducationYear,
                    LocationMaps = request.LocationMaps
                };

                var result = await _mediator.Send(command, cancellationToken);

                _logger.LogInformation(
                    "Instructor Google login successful. UserId: {UserId}, IsNewUser: {IsNewUser}",
                    result.UserId,
                    result.IsNewUser);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized instructor Google login attempt");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during instructor Google login");
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

                _logger.LogInformation("Instructor logout attempt for UserId: {UserId}", userId);

                var command = new GoogleLogoutCommand { UserId = userId };
                var result = await _mediator.Send(command, cancellationToken);

                _logger.LogInformation("Instructor logout successful for UserId: {UserId}", userId);

                return Ok(new { success = result, message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during instructor logout for UserId: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }
    }
}
