using Application.DTOs.Review;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Reviews.Commands.CreateReview
{
    public class CreateReviewCommand : IRequest<Result<ReviewResponse>>
    {
        public Guid StudentId { get; set; }
        public Guid EntityId { get; set; }
        public required ReviewEntityType EntityType { get; set; }
        public int StarRating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

}
