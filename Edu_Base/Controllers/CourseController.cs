using Application.Features.Course.Query.GetAllCourses;
using Application.Features.Course.Query.GetAllCoursesByInstructor;
using Application.Features.Course.Query.GetAllCoursesForStudent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController(IMediator mediator, ILogger<CourseController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<CourseController> _logger = logger;

        [HttpGet("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all courses");
                var query = new GetAllCoursesQuery();
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching all courses");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetAllCoursesEnrolledByStudent/{studentId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetAllCoursesEnrolledByStudent(Guid studentId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching courses for StudentId: {StudentId}", studentId);

                var query = new GetAllCoursesEnrolledByStudentQuery { StudentId = studentId };
                var result = await _mediator.Send(query, cancellationToken);

                return Ok(result);
            }
            catch (UnauthorizedAccessException auth)
            {
                _logger.LogWarning(auth, "Unauthorized access attempt for StudentId: {StudentId}", studentId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching courses for StudentId: {StudentId}", studentId);
            }
            return Ok("This endpoint is under construction.");
        }

        [HttpGet("GetAllCoursesByInstructor/{instructorId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetAllCoursesByInstructor(Guid instructorId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching courses for InstructorId: {InstructorId}", instructorId);
                var query = new GetAllCoursesByInstructorQuery { InstructorId = instructorId };
                var result = await _mediator.Send(query, cancellationToken);
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
