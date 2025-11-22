using Application.DTOs.Review;
using Application.ResultWrapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReviewService
    {
        Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default);
        Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default);
        Task DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    }
}
