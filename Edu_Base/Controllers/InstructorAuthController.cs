using Application.DTOs.Auth;
using Application.Features.Auth.Commands.GoogleLogout;
using Application.Features.Auth.Commands.InstructorGoogleLogin;
using Application.Features.Auth.Commands.StudentGoogleLogin;
using Application.Features.Auth.Commands.UserLoginWithRefreshToken;
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
                GoogleUserInfo = new GoogleUserInfo
                {
                    IdToken = request.IdToken,
                    FullName = request.FullName,
                    Email = request.Email,
                    PictureUrl = request.PictureUrl,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    PhoneNumber = request.PhoneNumber
                },
                Ssn = request.Ssn,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
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

            _logger.LogInformation("Instructor logout successful for UserId: {UserId}", userId);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);

        }

        [HttpPatch("update-token")]
        public async Task<IActionResult> UpdateJwtToken(LoginWithRefreshTokenCommand loginWithRefreshToken, CancellationToken cancellationToken)
        {
            if (loginWithRefreshToken == null)
                throw new ArgumentNullException(nameof(loginWithRefreshToken));

            var result = await _mediator.Send(loginWithRefreshToken, cancellationToken);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);

        }
    }
}
