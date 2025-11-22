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
    public class ReviewController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("createCourseReview")]
        public async Task<IActionResult> CreateCourseReview(ReviewCreationRequest reviewCreationRequest, CancellationToken cancellationToken)
        {
            if (reviewCreationRequest == null)
            {
                return BadRequest("Course review creation request can not be null.");
            }

            CreateReviewCommand reviewCommand = new()
            {
                Comment = reviewCreationRequest.Comment,
                StarRating = reviewCreationRequest.StarRating,
                EntityId = reviewCreationRequest.EntityId,
                StudentId = reviewCreationRequest.StudentId,
                EntityType = reviewCreationRequest.EntityType
            };

            var result = await _mediator.Send(reviewCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPut("updateCourseReview")]
        public async Task<IActionResult> UpdateCourseReview(ReviewResponse courseReviewUpdateRequest, CancellationToken cancellationToken)
        {
            if (courseReviewUpdateRequest == null)
            {
                return BadRequest("Course review update request can not be null.");
            }

            UpdateCourseReviewCommand updatedCourseReview = new()
            {
                CourseReviewId = courseReviewUpdateRequest.CourseReviewId,
                Comment = courseReviewUpdateRequest.Comment,
                StarRating = courseReviewUpdateRequest.StarRating,
            };

            var result = await _mediator.Send(updatedCourseReview, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpDelete("deleteCourseReview")]
        public async Task<IActionResult> DeleteCourseReview(Guid courseReviewId, CancellationToken cancellationToken)
        {
            if (courseReviewId == Guid.Empty)
            {
                return BadRequest("Course review delete request can not be null");
            }

            DeleteCourseReviewCommand deleteCourseReview = new()
            {
                CourseReviewId = courseReviewId
            };

            var result = await _mediator.Send(deleteCourseReview, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

    }
}
