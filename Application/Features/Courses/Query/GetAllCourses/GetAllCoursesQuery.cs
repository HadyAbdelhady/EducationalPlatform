using Application.DTOs;
using Application.DTOs.Courses;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Courses.Query.GetAllCourses
{
    public class GetAllCoursesQuery : IRequest<Result<PaginatedResult<CourseResponse>>>
    {
        public GetAllEntityRequestSkeleton GetAllEntityRequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid UserID { get; set; }

    }
}