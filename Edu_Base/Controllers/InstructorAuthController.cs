using Application.DTOs.Auth;
using Application.Features.Auth.Commands.GoogleLogout;
using Application.Features.Auth.Commands.InstructorGoogleLogin;
using Application.Features.Auth.Commands.InstructorGoogleSignUp;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstructorAuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<InstructorAuthController> _logger;

        public InstructorAuthController(IMediator mediator, ILogger<InstructorAuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("google-signup")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GoogleSignUp(
            [FromBody] InstructorGoogleLoginRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Instructor Google signup attempt");

                var command = new InstructorGoogleSignUpCommand
                {
                    IdToken = request.IdToken,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    EducationYear = request.EducationYear,
                    LocationMaps = request.LocationMaps
                };

                var result = await _mediator.Send(command, cancellationToken);

                _logger.LogInformation(
                    "Instructor Google signup successful. UserId: {UserId}",
                    result.UserId);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized instructor Google signup attempt");
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Instructor signup failed - user already exists");
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during instructor Google signup");
                return StatusCode(500, new { message = "An error occurred during signup" });
            }
        }

        [HttpPost("google-login")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
