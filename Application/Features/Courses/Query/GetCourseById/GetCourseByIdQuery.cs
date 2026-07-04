using Application.Features.Courses.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.Courses.Query.GetCourseById
{
    public class GetCourseByIdQuery : IRequest<Result<CourseDetailResponse>>
    {
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
    }
}
