using Application.DTOs.Auth;
using Application.Features.Auth.Commands.CenterAdminGoogleLogin;
using Application.Features.Auth.Commands.StudentGoogleLogin; // For GoogleUserInfo
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CenterAdminAuthController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] CenterAdminGoogleLoginRequest request, CancellationToken cancellationToken = default)
        {
            var command = new CenterAdminGoogleLoginCommand
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
                LocationMaps = request.LocationMaps,
                CenterId = request.CenterId
            };

            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode((int)result.ErrorType, result);
        }
    }
}
