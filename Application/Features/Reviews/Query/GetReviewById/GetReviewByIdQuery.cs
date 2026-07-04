using Application.Features.Reviews.DTOs;
using Application.Common;
using Domain.enums;
using MediatR;

namespace Application.Features.Review.Query.GetReviewById
{
    public class GetReviewByIdQuery : IRequest<Result<GetReviewByIdResponse>>
    {
        public Guid ReviewId { get; set; }

        public ReviewEntityType EntityType { get; set; }
    }
}
