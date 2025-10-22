using Application.DTOs.Course;
using Application.Features.Course.Commands.CreateCourse;
using Application.Features.Course.Commands.DeleteCourse;
using Application.Features.Course.Query.GetAllCourses;
using Application.Features.Course.Query.GetAllCoursesByInstructor;
using Application.Features.Course.Query.GetAllCoursesForStudent;
using Application.Features.Course.Query.GetCourseById;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Edu_Base.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController(IMediator mediator, ILogger<CourseController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<CourseController> _logger = logger;

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreationRequest courseCreationRequest, CancellationToken cancellationToken)
        {
            if (courseCreationRequest == null)
            {
                return BadRequest("Course creation request cannot be null.");
            }
            try
            {
                var createCourseCommand = new CreateCourseCommand
                {
                    CourseName = courseCreationRequest.CourseName,
                    Description = courseCreationRequest.Description,
                    InstructorId = courseCreationRequest.InstructorId,
                    Price = courseCreationRequest.Price,
                    PictureUrl = courseCreationRequest.PictureUrl,
                    IntroVideoUrl = courseCreationRequest.IntroVideoUrl
                };
                var result = await _mediator.Send(createCourseCommand, cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException auth)
            {
                _logger.LogWarning(auth, "Unauthorized access attempt to create course");
                return Unauthorized(new { message = auth.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetCourseDetailById/{courseId}")]
        public async Task<IActionResult> GetCourseDetailById(Guid courseId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching course detail for CourseId: {CourseId}", courseId);
                var query = new GetCourseByIdQuery { CourseId = courseId };
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching course detail for CourseId: {CourseId}", courseId);
                return StatusCode(500, "Internal server error");
            }
        }

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
        public async Task<IActionResult> GetAllCoursesEnrolledByStudent(Guid studentId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching courses for StudentId: {StudentId}", studentId);

                var query = new GetAllCoursesEnrolledByStudentQuery { StudentId = studentId };
                var result = await _mediator.Send(query, cancellationToken);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching courses for StudentId: {StudentId}", studentId);
            }
            return Ok("This endpoint is under construction.");
        }

        [HttpGet("GetAllCoursesByInstructor/{instructorId}")]
        public async Task<IActionResult> GetAllCoursesByInstructor(Guid instructorId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching courses for InstructorId: {InstructorId}", instructorId);
                var query = new GetAllCoursesByInstructorQuery { InstructorId = instructorId };
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching courses for InstructorId: {InstructorId}", instructorId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteCourse(Guid courseId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting course with CourseId: {CourseId}", courseId);
                var command = new DeleteCourseCommand
                {
                    CourseId = courseId
                };
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
                _logger.LogError(ex, "Error Deleting the course");
                return StatusCode(500, "Internal server error");
            }


        }
    }
}
