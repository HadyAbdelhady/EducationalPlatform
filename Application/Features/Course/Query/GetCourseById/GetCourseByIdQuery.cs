using Application.DTOs.Course;
using MediatR;

namespace Application.Features.Course.Query.GetCourseById
{
    public class GetCourseByIdQuery : IRequest<CourseDetailResponse>
    {
        public Guid CourseId { get; set; }
    }
}
