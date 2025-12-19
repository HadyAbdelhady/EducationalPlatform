using Application.DTOs.Courses;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Courses.Query.GetAllCourses
{
    public class GetAllCoursesQuery : IRequest<Result<PaginatedResult<CourseByUserIdResponse>>>
    {
    }
}