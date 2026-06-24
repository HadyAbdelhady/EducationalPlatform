using Application.DTOs.Review;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Reviews.Query.CheckReviewExists
{
    public class CheckReviewExistsQuery : IRequest<Result<ReviewResponse?>>
    {
        public Guid EntityId { get; set; }
        public ReviewEntityType EntityType { get; set; }
        public Guid StudentId { get; set; }
    }
}
