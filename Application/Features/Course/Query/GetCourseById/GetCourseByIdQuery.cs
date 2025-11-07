using Application.DTOs.Course;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Course.Query.GetCourseById
{
    public class GetCourseByIdQuery : IRequest<Result<CourseDetailResponse>>
    {
        public Guid CourseId { get; set; }
    }
}
