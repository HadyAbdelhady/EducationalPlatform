using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Review.Commands.DeleteReview
{
    public class DeleteCourseReviewCommand : IRequest<Result<string>>
    {
        public Guid CourseReviewId { get; set; }
    }
}
