using Application.Features.Auth.Queries.CheckUserExists;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedAuthController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Checks if a user already exists in the system by their email.
        /// Useful for the frontend to determine whether to route to a Login or a Registration flow.
        /// Returns the user data if they exist, or null if they are not found.
        /// </summary>
        [HttpGet("check-user")]
        public async Task<IActionResult> CheckUserExists([FromQuery] string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email parameter is required.");
            }

            var query = new CheckUserExistsQuery { Email = email };
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return StatusCode((int)result.ErrorType, result);
        }
    }
}
