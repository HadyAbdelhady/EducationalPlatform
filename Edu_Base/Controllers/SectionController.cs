using Application.DTOs.Section;
using Application.Features.Section.Commands.CreateSection;
using Application.Features.Section.Commands.DeleteSection;
using Application.Features.Section.Commands.UpdateSection;
using Application.Features.Section.Query.GetSectionsForCourse;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("create")]
        public async Task<IActionResult> CreateSection(CreateSectionRequest sectionCreationRequest, CancellationToken cancellationToken)
        {
            if (sectionCreationRequest == null)
            {
                return BadRequest("Section creation request cannot be null.");
            }
            var createSectionCommand = new CreateSectionCommand
            {
                Name = sectionCreationRequest.Name,
                Description = sectionCreationRequest.Description,
                Price = sectionCreationRequest.Price,
                NumberOfVideos = 0,
                CourseId = sectionCreationRequest.CourseId
            };
            var result = await _mediator.Send(createSectionCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPost("bulk-create")]
        public async Task<IActionResult> BulkCreateSections(BulkCreateSectionRequest request, CancellationToken cancellationToken)
        {
            var command = new BulkCreateSectionCommand(request.Sections);

            var result = await _mediator.Send(command, cancellationToken);

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetSectionsForCourse(Guid courseId)
        {
            var result = await _mediator.Send(new GetSectionsForCourseQuery(courseId));

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSection(SectionUpdateRequest sectionUpdateRequest, CancellationToken cancellationToken)
        {
            if (sectionUpdateRequest == null)
                return BadRequest("Update request cannot be null.");

            var updateCommand = new UpdateSectionCommand
            {
                SectionId = sectionUpdateRequest.SectionId,
                Name = sectionUpdateRequest.Name,
                Description = sectionUpdateRequest.Description,
                Price = sectionUpdateRequest.Price,
                CourseId = sectionUpdateRequest.CourseId
            };

            var result = await _mediator.Send(updateCommand, cancellationToken);

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteSection(Guid id, CancellationToken cancellationToken)
        {
            var deleteCommand = new DeleteSectionCommand
            {
                SectionId = id
            };

            var result = await _mediator.Send(deleteCommand, cancellationToken);

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpDelete("bulk-delete")]
        public async Task<IActionResult> BulkDeleteSections(BulkDeleteSectionRequest request, CancellationToken cancellationToken)
        {
            var command = new BulkDeleteSectionCommand(request.SectionIds);

            var result = await _mediator.Send(command, cancellationToken);

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

    }
}
