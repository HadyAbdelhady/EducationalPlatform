using Application.Common;
using Application.Features.Reviews.DTOs;
using Domain.enums;
using MediatR;

namespace Application.Features.Reviews.Query.GetAllReviews
{
    public class GetAllReviewsQuery : IRequest<Result<PaginatedResult<GetAllReviewsResponse>>>
    {
        public Guid EntityId { get; set; }
        public ReviewEntityType EntityType { get; set; }

        public GetAllEntityRequestSkeleton GetAllEntityRequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();

    }
}
