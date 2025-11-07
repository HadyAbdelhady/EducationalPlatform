using Application.DTOs.Course;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Course.Query.GetAllCourses
{
    public class GetAllCoursesQuery : IRequest<Result<PaginatedResult<CourseByUserIdResponse>>>
    {
    }
}