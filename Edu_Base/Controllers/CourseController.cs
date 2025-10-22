using Application.Features.Course.Commands.GetAllCoursesByInstructor;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Edu_Base.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController(IMediator mediator, ILogger<CourseController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<CourseController> _logger = logger;

        [HttpGet("{instructorId}")]
        public async Task<IActionResult> GetAllCoursesByInstructor(Guid instructorId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching courses for InstructorId: {InstructorId}", instructorId);
                var command = new GetAllCoursesByInstructorQuery { InstructorId = instructorId };
                var result = await _mediator.Send(command, cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching courses for InstructorId: {InstructorId}", instructorId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
