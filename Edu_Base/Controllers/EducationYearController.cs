using Application.Features.EducationYears.Queries.GetEducationYears;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationYearController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetEducationYears(CancellationToken cancellationToken = default)
        {
            var query = new GetEducationYearsQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }
    }
}
