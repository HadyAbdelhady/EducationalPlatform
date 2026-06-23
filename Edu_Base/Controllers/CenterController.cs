using Application.DTOs.Center;
using Application.Features.Centers.Commands.AssignInstructorToCenter;
using Application.Features.Centers.Commands.CreateCenter;
using Application.Features.Centers.Commands.DeleteCenter;
using Application.Features.Centers.Commands.RemoveInstructorFromCenter;
using Application.Features.Centers.Commands.UpdateCenter;
using Application.Features.Centers.Queries.GetAllCenters;
using Application.Features.Centers.Queries.GetCenterById;
using Application.ResultWrapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    /// <summary>
    /// Manages Center entities. All write operations require the CenterAdmin role.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CenterController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        // ── Queries ───────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAllCentersQuery(), cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCenterByIdQuery { Id = id }, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }

        // ── Commands (CenterAdmin only) ───────────────────────────────────────────

        [HttpPost]
        [Authorize(Roles = "CenterAdmin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateCenterRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CreateCenterCommand { Request = request }, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "CenterAdmin")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateCenterRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new UpdateCenterCommand { Id = id, Request = request }, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "CenterAdmin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new DeleteCenterCommand { Id = id }, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }

        [HttpPost("{centerId:guid}/instructors")]
        [Authorize(Roles = "CenterAdmin")]
        public async Task<IActionResult> AssignInstructor(
            Guid centerId,
            [FromBody] AssignInstructorToCenterRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new AssignInstructorToCenterCommand { CenterId = centerId, Request = request },
                cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }

        [HttpDelete("{centerId:guid}/instructors/{instructorId:guid}")]
        [Authorize(Roles = "CenterAdmin")]
        public async Task<IActionResult> RemoveInstructor(
            Guid centerId,
            Guid instructorId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new RemoveInstructorFromCenterCommand { CenterId = centerId, InstructorId = instructorId },
                cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }
    }
}
