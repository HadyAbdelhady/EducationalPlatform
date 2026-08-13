using Application.Common.Interfaces;
using Application.Features.EducationYears.DTOs;
using Application.Features.EducationYears.Commands.CreateEducationYear;
using Application.Features.EducationYears.Commands.DeleteEducationYear;
using Application.Features.EducationYears.Commands.UpdateEducationYear;
using Application.Features.EducationYears.Queries.GetEducationYearById;
using Application.Features.EducationYears.Queries.GetEducationYears;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Features.EducationYears
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationYearController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ICurrentUserService _currentUser = currentUser;

        [HttpGet]
        public async Task<IActionResult> GetEducationYears([FromQuery] string? ApplicationName, CancellationToken cancellationToken = default)
        {
            var query = new GetEducationYearsQuery { InstructorId = null, ApplicationName = ApplicationName };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetEducationYearById(Guid id, CancellationToken cancellationToken = default)
        {
            var query = new GetEducationYearByIdQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEducationYear([FromBody] CreateEducationYearRequest request, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.TryGetUserId(out var userId))
                return Unauthorized("User id not found in token.");

            var command = new CreateEducationYearCommand { EducationYear = request, InstructorId = userId };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEducationYear(Guid id, [FromBody] UpdateEducationYearRequest request, CancellationToken cancellationToken = default)
        {
            var command = new UpdateEducationYearCommand { Id = id, EducationYear = request };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEducationYear(Guid id, CancellationToken cancellationToken = default)
        {
            var command = new DeleteEducationYearCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }
    }
}
