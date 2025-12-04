using Application.DTOs.Review;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Review.Query.GetAllReviewsByCourse
{
    public class GetAllReviewsByCourseQuery : IRequest<Result<List<GetAllReviewsByCourseResponse>>>
    {
        public Guid CourseId { get; set; }
    }
}

