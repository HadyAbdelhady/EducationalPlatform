using Application.Features.Reviews.DTOs;
using Application.Common.Interfaces;
using Application.Features.Reviews.Interfaces;
using Application.Common;
using MediatR;

namespace Application.Features.Reviews.Query.CheckReviewExists
{
    public class CheckReviewExistsQueryHandler(IReviewServiceFactory reviewServiceFactory) : IRequestHandler<CheckReviewExistsQuery, Result<ReviewResponse?>>
    {
        private readonly IReviewServiceFactory _reviewServiceFactory = reviewServiceFactory;

        public async Task<Result<ReviewResponse?>> Handle(CheckReviewExistsQuery request, CancellationToken cancellationToken)
        {
            IReviewService reviewService = _reviewServiceFactory.GetReviewService(request.EntityType);
            return await reviewService.DoesReviewExist(request.StudentId, request.EntityId, cancellationToken);
        }
    }
}
