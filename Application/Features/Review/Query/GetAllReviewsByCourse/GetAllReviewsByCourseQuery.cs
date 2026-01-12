using Application.DTOs.Review;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Reviews.Query.GetAllReviewsByCourse
{
    public class GetAllReviewsByCourseQuery : IRequest<Result<List<GetAllReviewsByCourseResponse>>>
    {
        public Guid CourseId { get; set; }
    }
}

