using Application.Common.Interfaces;
using Application.Features.Search.Queries.StudentSearch;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Features.Search
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController(
        IMediator mediator,
        ICurrentUserService currentUser) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ICurrentUserService _currentUser = currentUser;

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string q,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUser.TryGetUserId(out var userId))
                return Unauthorized();

            var query = new StudentSearchQuery
            {
                UserId = userId,
                Query = q ?? string.Empty,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }
    }
}
