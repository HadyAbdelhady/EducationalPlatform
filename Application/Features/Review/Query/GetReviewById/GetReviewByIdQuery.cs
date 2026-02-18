using Application.DTOs.Review;
using Application.ResultWrapper;
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
