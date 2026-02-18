using Application.DTOs.Review;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Reviews.Commands.UpdateReview
{
    public class UpdateReviewCommand : IRequest<Result<ReviewResponse>>
    {
        public Guid ReviewId { get; set; }
        public required ReviewEntityType EntityType { get; set; }
        public int StarRating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
