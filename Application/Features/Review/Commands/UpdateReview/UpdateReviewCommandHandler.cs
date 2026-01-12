using Application.DTOs.Review;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Reviews.Commands.UpdateReview
{
    public class UpdateReviewCommandHandler(IReviewServiceFactory reviewServiceFactory) : IRequestHandler<UpdateReviewCommand, Result<ReviewResponse>>
    {
        private readonly IReviewServiceFactory _reviewServiceFactory = reviewServiceFactory;

        public async Task<Result<ReviewResponse>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            IReviewService reviewService = _reviewServiceFactory.GetReviewService(request.EntityType);

            return await reviewService.UpdateReviewAsync(new ReviewUpdateRequest
            {
                Comment = request.Comment,
                EntityType = request.EntityType,
                ReviewId = request.ReviewId,
                StarRating = request.StarRating,
            }, cancellationToken); ;
        }
    }
}
