using Application.DTOs.Review;
using Application.Interfaces;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Reviews.Query.CheckReviewExists
{
    public class CheckReviewExistsQueryHandler(IReviewServiceFactory reviewServiceFactory) : IRequestHandler<CheckReviewExistsQuery, Result<ReviewResponse?>>
    {
        private readonly IReviewServiceFactory _reviewServiceFactory = reviewServiceFactory;

        public async Task<Result<ReviewResponse?>> Handle(CheckReviewExistsQuery request, CancellationToken cancellationToken)
        {
            IReviewService reviewService = _reviewServiceFactory.GetReviewService(request.EntityType);
            return await reviewService.IsReviewExists(request.StudentId, request.EntityId, cancellationToken);
        }
    }
}
