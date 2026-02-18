using Application.DTOs.Auth;
using Application.Features.Auth.Commands.GoogleLogout;
using Application.Features.Auth.Commands.StudentGoogleLogin;
using Application.Features.Auth.Commands.UserLoginWithRefreshToken;
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
            _logger.LogInformation("Student Google signup/login attempt for device: {DeviceId}", request.DeviceId);

            // Use the consolidated login command which handles both signup and login
            var command = new StudentGoogleLoginCommand
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
                ParentPhoneNumber = request.ParentPhoneNumber,
                DeviceId = request.DeviceId,
                Ssn = request.Ssn,
                EducationYearId = request.EducationYearId,
                LocationMaps = request.LocationMaps
            };

            var result = await _mediator.Send(command, cancellationToken);


            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);

        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] Guid userId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }

            _logger.LogInformation("Student logout attempt for UserId: {UserId}", userId);

            var command = new GoogleLogoutCommand { UserId = userId };
            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Student logout successful for UserId: {UserId}", userId);

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
