using Application.DTOs.Courses;
using Application.Features.Courses.Commands.CreateCourse;
using Application.Features.Courses.Commands.DeleteCourse;
using Application.Features.Courses.Commands.UpdateCourse;
using Application.Features.Courses.Query.GetAllCourses;
using Application.Features.Courses.Query.GetCourseById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

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
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);

        }

        [HttpGet("GetCourseDetailById/{courseId}")]
        public async Task<IActionResult> GetCourseDetailById(Guid courseId, CancellationToken cancellationToken)
        {
            var UserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            //Guid UserId = Guid.Parse("d446bb09-477d-4c9e-b6fe-6971e6c80dc5");

            var query = new GetCourseByIdQuery { CourseId = courseId, UserId = UserId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpGet("GetCoursesList")]
        public async Task<IActionResult> GetCoursesList([FromQuery] GetAllCoursesRequest request, CancellationToken cancellationToken)
        {
            var UserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            //Guid UserId = Guid.Parse("d446bb09-477d-4c9e-b6fe-6971e6c80dc5");
            var query = new GetAllCoursesQuery
            {
                Filters = request.Filters,
                SortBy = request.SortBy,
                IsDescending = request.IsDescending,
                UserID = UserId,
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateCourse(CourseUpdateRequest courseUpdateRequest, CancellationToken cancellationToken)
        {
            var command = new UpdateCourseCommand
            {
                Id = courseUpdateRequest.CourseId,
                CourseName = courseUpdateRequest.CourseName,
                Description = courseUpdateRequest.Description,
                InstructorId = courseUpdateRequest.InstructorId,
                Price = courseUpdateRequest.Price,
                PictureUrl = courseUpdateRequest.PictureUrl,
                IntroVideoUrl = courseUpdateRequest.IntroVideoUrl
            };
            var result = await _mediator.Send(command, cancellationToken);

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpDelete]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteCourse(Guid courseId, CancellationToken cancellationToken)
        {
            var command = new DeleteCourseCommand
            {
                CourseId = courseId
            };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }
    }
}
