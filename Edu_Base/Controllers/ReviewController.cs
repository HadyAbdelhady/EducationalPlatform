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

        [HttpPost("createReview")]
        public async Task<IActionResult> CreateCourseReview(ReviewCreationRequest reviewCreationRequest, CancellationToken cancellationToken)
        {
            if (reviewCreationRequest is null)
            {
                return BadRequest("Review creation request can not be null.");
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

        [HttpPut("updateReview")]
        public async Task<IActionResult> UpdateCourseReview(ReviewUpdateRequest reviewUpdateRequest, CancellationToken cancellationToken)
        {
            if (reviewUpdateRequest is null)
            {
                return BadRequest("Review update request can not be null.");
            }

            UpdateReviewCommand updatedCourseReview = new()
            {
                ReviewId = reviewUpdateRequest.ReviewId,
                EntityType = reviewUpdateRequest.EntityType,
                Comment = reviewUpdateRequest.Comment,
                StarRating = reviewUpdateRequest.StarRating,
            };

            var result = await _mediator.Send(updatedCourseReview, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpDelete("deleteReview")]
        public async Task<IActionResult> DeleteCourseReview(ReviewDeletionRequest reviewDeletionRequest, CancellationToken cancellationToken)
        {
            if (reviewDeletionRequest is null)
            {
                return BadRequest("Review deletion request can not be null");
            }

            DeleteReviewCommand deleteReview = new()
            {
                ReviewId = reviewDeletionRequest.ReviewId,
                EntityType = reviewDeletionRequest.EntityType,
            };

            var result = await _mediator.Send(deleteReview, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

    }
}
