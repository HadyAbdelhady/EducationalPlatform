using Application.DTOs.Review;
using Application.Features.Review.Commands.CreateReview;
using Application.Features.Review.Commands.DeleteReview;
using Application.Features.Review.Commands.UpdateReview;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController(IMediator mediator, ILogger<ReviewController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("createCourseReview")]
        public async Task<IActionResult> CreateCourseReview(CourseReviewCreationRequest courseReviewCreationRequest, CancellationToken cancellationToken)
        {
            if (courseReviewCreationRequest == null)
            {
                return BadRequest("Course review creation request can not be null.");
            }

            CreateCourseReviewCommand courseReviewCommand = new CreateCourseReviewCommand
            {
                Comment = courseReviewCreationRequest.Comment,
                StarRating = courseReviewCreationRequest.StarRating,
                CourseId = courseReviewCreationRequest.CourseId,
                StudentId = courseReviewCreationRequest.StudentId,
            };

            var result = await _mediator.Send(courseReviewCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPut("updateCourseReview")]
        public async Task<IActionResult> UpdateCourseReview(CourseReviewUpdateRequest courseReviewUpdateRequest, CancellationToken cancellationToken)
        {
            if(courseReviewUpdateRequest == null)
            {
                return BadRequest("Course review update request can not be null.");
            }

            UpdateCourseReviewCommand updatedCourseReview = new UpdateCourseReviewCommand
            {
                CourseReviewId = courseReviewUpdateRequest.CourseReviewId,
                Comment = courseReviewUpdateRequest.Comment,
                StarRating = courseReviewUpdateRequest.StarRating,
            };

            var result = await _mediator.Send(updatedCourseReview, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpDelete("deleteCourseReview")]
        public async Task<IActionResult> DeleteCourseReview(CourseReviewDeleteRequest courseReviewDeleteRequest, CancellationToken cancellationToken)
        {
            if (courseReviewDeleteRequest == null)
            {
                return BadRequest("Course review delete request can not be null");
            }

            DeleteCourseReviewCommand deleteCourseReview = new DeleteCourseReviewCommand
            {
                CourseReviewId = courseReviewDeleteRequest.CourseReviewId
            };

            var result = await _mediator.Send(deleteCourseReview, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result); 
        }

    }
}
