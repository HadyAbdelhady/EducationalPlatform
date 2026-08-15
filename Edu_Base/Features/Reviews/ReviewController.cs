using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Reviews.DTOs;
using Application.Features.Review.Query.GetAllReviews;
using Application.Features.Review.Query.GetReviewById;
using Application.Features.Reviews.Commands.CreateReview;
using Application.Features.Reviews.Commands.DeleteReview;
using Application.Features.Reviews.Commands.UpdateReview;
using Application.Features.Reviews.Query.CheckReviewExists;
using Application.Features.Reviews.Query.GetAllReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Features.Reviews
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ICurrentUserService _currentUser = currentUser;

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreateReview(ReviewCreationRequest reviewCreationRequest, CancellationToken cancellationToken)
        {
            if (reviewCreationRequest is null)
            {
                return BadRequest("Review creation request can not be null.");
            }

            if (!_currentUser.TryGetUserId(out var userId))
                return Unauthorized("User id not found in token.");

            CreateReviewCommand reviewCommand = new()
            {
                Comment = reviewCreationRequest.Comment,
                StarRating = reviewCreationRequest.StarRating,
                EntityId = reviewCreationRequest.EntityId,
                StudentId = userId,
                EntityType = reviewCreationRequest.EntityType
            };

            var result = await _mediator.Send(reviewCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPatch]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateReview(ReviewUpdateRequest reviewUpdateRequest, CancellationToken cancellationToken)
        {
            if (reviewUpdateRequest is null)
            {
                return BadRequest("Review update request can not be null.");
            }

            if (!_currentUser.TryGetUserId(out var userId))
                return Unauthorized("User id not found in token.");

            UpdateReviewCommand updatedCourseReview = new()
            {
                ReviewId = reviewUpdateRequest.ReviewId,
                EntityType = reviewUpdateRequest.EntityType,
                Comment = reviewUpdateRequest.Comment,
                StarRating = reviewUpdateRequest.StarRating,
                StudentId = userId,
            };

            var result = await _mediator.Send(updatedCourseReview, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpDelete]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> DeleteReview(ReviewDeletionRequest reviewDeletionRequest, CancellationToken cancellationToken)
        {
            if (reviewDeletionRequest is null)
            {
                return BadRequest("Review deletion request can not be null");
            }

            if (!_currentUser.TryGetUserId(out var userId))
                return Unauthorized("User id not found in token.");

            DeleteReviewCommand deleteReview = new()
            {
                ReviewId = reviewDeletionRequest.ReviewId,
                EntityType = reviewDeletionRequest.EntityType,
                StudentId = userId,
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
                GetAllEntityRequestSkeleton = request.GetAllEntityRequestSkeleton
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("CheckReviewExists")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CheckReviewExists([FromQuery] CheckReviewExistsQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out var userId))
                return Unauthorized("User id not found in token.");

            if (request.EntityId == Guid.Empty)
            {
                return BadRequest("Entity ID cannot be empty");
            }

            var query = new CheckReviewExistsQuery
            {
                EntityId = request.EntityId,
                StudentId = userId,
                EntityType = request.EntityType
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }
    }
}
