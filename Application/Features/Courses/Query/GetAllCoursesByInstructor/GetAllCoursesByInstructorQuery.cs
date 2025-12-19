using Application.DTOs.Courses;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Courses.Query.GetAllCoursesByInstructor
{
    public class GetAllCoursesByInstructorQuery : IRequest<Result<PaginatedResult<CourseByUserIdResponse>>>
    {
        public Guid InstructorId { get; set; }
    }
}
