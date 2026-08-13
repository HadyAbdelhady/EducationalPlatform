using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Courses.DTOs;
using Application.Features.Courses.Commands.CreateCourse;
using Application.Features.Courses.Commands.DeleteCourse;
using Application.Features.Courses.Commands.UpdateCourse;
using Application.Features.Courses.Query.GetAllCourses;
using Application.Features.Courses.Query.GetCourseById;
using Application.Features.Courses.Query.GetCourseNamesByInstructor;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Features.Courses
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ICurrentUserService _currentUser = currentUser;

        [HttpPost("create")]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateCourse([FromForm] CourseCreationRequest courseCreationRequest, CancellationToken cancellationToken)
        {
            if (courseCreationRequest == null)
            {
                return BadRequest("Course creation request cannot be null.");
            }
            var createCourseCommand = new CreateCourseCommand
            {
                CourseName = courseCreationRequest.CourseName,
                Description = courseCreationRequest.Description,
                EducationYearId = courseCreationRequest.EducationYearId,
                InstructorId = courseCreationRequest.InstructorId,
                Price = courseCreationRequest.Price,
                PictureUrl = courseCreationRequest.PictureUrl,
                IntroVideoUrl = courseCreationRequest.IntroVideoUrl,
                PictureFile = courseCreationRequest.PictureFile
            };
            var result = await _mediator.Send(createCourseCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);

        }

        [HttpGet("GetCourseDetailById/{courseId}")]
        public async Task<IActionResult> GetCourseDetailById(Guid courseId, CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out var userId))
                return Unauthorized("User id not found in token.");

            var query = new GetCourseByIdQuery { CourseId = courseId, UserId = userId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("GetCoursesList")]
        public async Task<IActionResult> GetCoursesList([FromQuery] GetAllEntityRequestSkeleton request, CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out var userId))
                return Unauthorized("User id not found in token.");

            var query = new GetAllCoursesQuery
            {
                GetAllEntityRequestSkeleton = request,
                UserID = userId,
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateCourse([FromForm] CourseUpdateRequest courseUpdateRequest, CancellationToken cancellationToken)
        {
            var command = new UpdateCourseCommand
            {
                Id = courseUpdateRequest.CourseId,
                CourseName = courseUpdateRequest.CourseName,
                Description = courseUpdateRequest.Description,
                EducationYearId = courseUpdateRequest.EducationYearId,
                InstructorId = courseUpdateRequest.InstructorId,
                //Price = courseUpdateRequest.Price,
                PictureUrl = courseUpdateRequest.PictureUrl,
                IntroVideoUrl = courseUpdateRequest.IntroVideoUrl,
                PictureFile = courseUpdateRequest.PictureFile
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
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("GetCourseNamesByInstructor")]
        public async Task<IActionResult> GetCourseNamesByInstructor([FromQuery] Guid EducationalYearId, CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out var userId))
                return Unauthorized("User id not found in token.");

            var query = new GetCourseNamesByInstructorQuery { InstructorId = userId, EducationalYearId = EducationalYearId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }
    }
}
