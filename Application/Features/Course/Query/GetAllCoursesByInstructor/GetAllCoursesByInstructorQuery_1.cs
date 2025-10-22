using Application.DTOs.Course;
using MediatR;

namespace Application.Features.Course.Query.GetAllCoursesByInstructor
{
    public class GetAllCoursesByInstructorQuery : IRequest<List<CourseByInstructorResponse>>
    {
        public Guid InstructorId { get; set; }
    }
}
