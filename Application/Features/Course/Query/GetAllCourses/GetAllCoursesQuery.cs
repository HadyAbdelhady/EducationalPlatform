using Application.DTOs.Course;
using MediatR;

namespace Application.Features.Course.Query.GetAllCourses
{
    public class GetAllCoursesQuery : IRequest<List<CourseByUserIdResponse>>
    {
    }
}