using Application.DTOs.Review;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;

namespace Infrastructure.Services.ReviewService
{

    public class ReviewServiceBase<TReview>(IUnitOfWork unitOfWork) : IReviewService where TReview : Review, new()
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public virtual async Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var student = await _unitOfWork.Repository<User>().AnyAsync(s => s.Id == request.StudentId, cancellationToken);
                if (!student)
                {
                    return Result<ReviewResponse>.FailureStatusCode("Student not found.", ErrorType.NotFound);
                }

                var reviewAlreadyExists = await _unitOfWork.Repository<TReview>().AnyAsync(r => r.StudentId == request.StudentId &&
                                                                                           r.EntityId == request.EntityId,
                                                                                           cancellationToken);
                if (reviewAlreadyExists)
                {
                    return Result<ReviewResponse>.FailureStatusCode("You have already submitted a review.", ErrorType.BadRequest);
                }

                TReview newReview = new()
                {
                    Id = Guid.NewGuid(),
                    Comment = request.Comment,
                    StarRating = request.StarRating,
                    StudentId = request.StudentId,
                    EntityId = request.EntityId
                };

                await _unitOfWork.Repository<TReview>().AddAsync(newReview, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<ReviewResponse>.Success(new ReviewResponse
                {
                    ReviewId = newReview.Id,
                    Comment = request.Comment,
                    StarRating = request.StarRating

                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<ReviewResponse>.FailureStatusCode($"Unauthorized access: {ex.Message}", ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<ReviewResponse>.FailureStatusCode($"An error occurred while creating the review.{ex.Message}", ErrorType.InternalServerError);
            }
        }

        public virtual async Task<Result<string>> DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
        {
            try
            {
                var reviewExists = await _unitOfWork.Repository<TReview>().AnyAsync(r => r.Id == reviewId, cancellationToken);
                if (!reviewExists)
                {
                    return Result<string>.FailureStatusCode($"Review not found", ErrorType.NotFound);
                }

                await _unitOfWork.Repository<TReview>().RemoveAsync(reviewId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<string>.Success($"Review with ID: {reviewId} deleted Successfully.");
            }
            catch (UnauthorizedAccessException authEx)
            {
                return Result<string>.FailureStatusCode(authEx.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<string>.FailureStatusCode($"Error deleting review: {ex.Message}", ErrorType.InternalServerError);
            }
        }

        public async Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var review = await _unitOfWork.Repository<TReview>().GetByIdAsync(request.ReviewId, cancellationToken);
            if (review is null)
            {
                return Result<ReviewResponse>.FailureStatusCode($"Review not found", ErrorType.NotFound);
            }

            review.Comment = request.Comment;
            review.StarRating = request.StarRating;
            review.UpdatedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Repository<TReview>().Update(review);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ReviewResponse>.Success(new ReviewResponse
            {
                ReviewId = request.ReviewId,
                StarRating = request.StarRating,
                Comment = request.Comment,
            });
        }
    }
}
