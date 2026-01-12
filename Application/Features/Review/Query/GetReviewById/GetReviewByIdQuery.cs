using Application.DTOs.Review;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Reviews.Query.GetReviewById
{
    public class GetReviewByIdQuery : IRequest<Result<GetReviewByIdResponse>>
    {
        public Guid ReviewId { get; set; }
    }
}
