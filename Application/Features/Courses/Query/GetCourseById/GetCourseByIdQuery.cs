using Application.DTOs.Courses;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Courses.Query.GetCourseById
{
    public class GetCourseByIdQuery : IRequest<Result<CourseDetailResponse>>
    {
        public Guid CourseId { get; set; }
    }
}
