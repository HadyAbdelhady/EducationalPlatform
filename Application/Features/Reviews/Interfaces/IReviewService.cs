using Application.Features.Reviews.DTOs;
using Application.Common;

namespace Application.Features.Reviews.Interfaces
{
    public interface IReviewService
    {
        Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default);
        Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default);
        Task<Result<string>> DeleteReviewAsync(Guid reviewId, Guid studentId, CancellationToken cancellationToken = default);
        Task<Result<PaginatedResult<GetAllReviewsResponse>>> GetAllReviewsAsync(ReviewGettingRequest request, CancellationToken cancellationToken = default);
        Task<Result<GetReviewByIdResponse>> GetReviewByIdAsync(Guid reviewId, CancellationToken cancellationToken = default);
        Task<Result<ReviewResponse?>> DoesReviewExist(Guid studentId, Guid entityId, CancellationToken cancellationToken = default);
    }
}
