using Application.DTOs.Review;
using Application.Features.Review.Query.GetAllReviews;
using Application.Features.Review.Query.GetReviewById;
using Application.Features.Reviews.Commands.CreateReview;
using Application.Features.Reviews.Commands.DeleteReview;
using Application.Features.Reviews.Commands.UpdateReview;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> CreateReview(ReviewCreationRequest reviewCreationRequest, CancellationToken cancellationToken)
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

        [HttpPatch]
        public async Task<IActionResult> UpdateReview(ReviewUpdateRequest reviewUpdateRequest, CancellationToken cancellationToken)
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

        [HttpDelete]
        public async Task<IActionResult> DeleteReview(ReviewDeletionRequest reviewDeletionRequest, CancellationToken cancellationToken)
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

        [HttpGet]
        public async Task<IActionResult> GetReviewById([FromQuery] GetReviewByIdRequest request, CancellationToken cancellationToken)
        {
            if (request.reviewId == Guid.Empty)
            {
                return BadRequest("Review ID cannot be empty");
            }

            var query = new GetReviewByIdQuery
            {
                ReviewId = request.reviewId,
                EntityType = request.EntityType
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("GetAllReviewsInEntity")]
        public async Task<IActionResult> GetAllReviews([FromQuery] ReviewGettingRequest request, CancellationToken cancellationToken)
        {
            if (request.EntityId == Guid.Empty)
            {
                return BadRequest("Entity ID cannot be empty");
            }

            var query = new GetAllReviewsQuery
            {
                EntityId = request.EntityId,
                EntityType = request.EntityType,
                Filters = request.Filters,
                SortBy = request.SortBy,
                IsDescending = request.IsDescending
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

    }

}
