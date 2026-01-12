using Application.DTOs.Review;
using Application.ResultWrapper;

namespace Application.Interfaces
{
    public interface IReviewService
    {
        Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default);
        Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default);
        Task<Result<string>> DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
        Task<Result<List<GetAllReviewsResponse>>> GetAllReviewsAsync(Guid entityId, CancellationToken cancellationToken = default);
    }
}
