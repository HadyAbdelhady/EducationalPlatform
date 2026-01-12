using Application.Features.Review.Query.GetAllReviews;
using Application.Features.Review.Commands.CreateReview;
using Application.Features.Review.Commands.DeleteReview;
using Application.Features.Review.Commands.UpdateReview;
using Application.Features.Review.Query.GetReviewById;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Review;
using Domain.enums;
using MediatR;


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

        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetReviewById(Guid reviewId, CancellationToken cancellationToken)
        {
            if (reviewId == Guid.Empty)
            {
                return BadRequest("Review ID cannot be empty");
            }
            var query = new GetReviewByIdQuery { ReviewId = reviewId };
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
                EntityType = request.EntityType 
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

    }

}
