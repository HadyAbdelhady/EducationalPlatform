using Application.DTOs;
using Application.DTOs.Review;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Reviews.Query.GetAllReviews
{
    public class GetAllReviewsQuery : IRequest<Result<List<GetAllReviewsResponse>>>
    {
        public Guid EntityId { get; set; }
        public ReviewEntityType EntityType { get; set; }

        public GetAllEntityRequestSkeleton GetAllEntityRequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();

    }
}
