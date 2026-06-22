using Application.DTOs.EducationYear;
using Application.Features.EducationYears.Commands.CreateEducationYear;
using Application.Features.EducationYears.Commands.DeleteEducationYear;
using Application.Features.EducationYears.Commands.UpdateEducationYear;
using Application.Features.EducationYears.Queries.GetEducationYearById;
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
        public async Task<IActionResult> GetEducationYears([FromQuery] string? ApplicationName, CancellationToken cancellationToken = default)
        {
            //var UserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            //Guid UserId = Guid.Parse("2cfd78cb-f08e-4ebe-a667-92eb59b9c19d");

            var query = new GetEducationYearsQuery { InstructorId = null, ApplicationName = ApplicationName };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetEducationYearById(Guid id, CancellationToken cancellationToken = default)
        {
            var query = new GetEducationYearByIdQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEducationYear([FromBody] CreateEducationYearRequest request, CancellationToken cancellationToken = default)
        {
            var UserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            //Guid UserId = Guid.Parse("57786109-6eea-45f6-9f6f-a45dbcc1b62f");

            var command = new CreateEducationYearCommand { EducationYear = request, InstructorId = UserId };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEducationYear(Guid id, [FromBody] UpdateEducationYearRequest request, CancellationToken cancellationToken = default)
        {
            var command = new UpdateEducationYearCommand { Id = id, EducationYear = request };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEducationYear(Guid id, CancellationToken cancellationToken = default)
        {
            var command = new DeleteEducationYearCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }
    }
}
