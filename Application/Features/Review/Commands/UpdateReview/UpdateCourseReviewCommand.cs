using Application.DTOs.Review;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Review.Commands.UpdateReview
{
    public class UpdateCourseReviewCommand : IRequest<Result<ReviewResponse>>
    {
        public Guid CourseReviewId { get; set; }
        public int StarRating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
