using Application.DTOs.Course;
using Application.Features.Course.Commands.CreateCourse;
using Application.Features.Course.Commands.DeleteCourse;
using Application.Features.Course.Query.GetAllCourses;
using Application.Features.Course.Query.GetAllCoursesByInstructor;
using Application.Features.Course.Query.GetAllCoursesForStudent;
using Application.Features.Course.Query.GetCourseById;
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

        [HttpPost("create")]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateCourse(CourseCreationRequest courseCreationRequest, CancellationToken cancellationToken)
        {
            if (courseCreationRequest == null)
            {
                return BadRequest("Course creation request cannot be null.");
            }
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
            return result.IsSuccess ? Ok(result.Value) : StatusCode((int)result.ErrorType, result.Error);

        }

        [HttpGet("GetCourseDetailById/{courseId}")]
        public async Task<IActionResult> GetCourseDetailById(Guid courseId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching course detail for CourseId: {CourseId}", courseId);
            var query = new GetCourseByIdQuery { CourseId = courseId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpGet("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all courses");
            var query = new GetAllCoursesQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpGet("GetAllCoursesEnrolledByStudent/{studentId}")]
        public async Task<IActionResult> GetAllCoursesEnrolledByStudent(Guid studentId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching courses for StudentId: {StudentId}", studentId);
            var query = new GetAllCoursesEnrolledByStudentQuery { StudentId = studentId };
            var result = await _mediator.Send(query, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpGet("GetAllCoursesByInstructor/{instructorId}")]
        public async Task<IActionResult> GetAllCoursesByInstructor(Guid instructorId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching courses for InstructorId: {InstructorId}", instructorId);
            var query = new GetAllCoursesByInstructorQuery { InstructorId = instructorId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteCourse(Guid courseId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting course with CourseId: {CourseId}", courseId);
            var command = new DeleteCourseCommand
            {
                CourseId = courseId
            };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }
    }
}
