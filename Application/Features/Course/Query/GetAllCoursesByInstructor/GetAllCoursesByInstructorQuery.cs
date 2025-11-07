using Application.DTOs.Course;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Course.Query.GetAllCoursesByInstructor
{
    public class GetAllCoursesByInstructorQuery : IRequest<Result<PaginatedResult<CourseByUserIdResponse>>>
    {
        public Guid InstructorId { get; set; }
    }
}
