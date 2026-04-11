using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Courses.Query.GetCourseNamesByInstructor
{
    public class GetCourseNamesByInstructorQuery : IRequest<Result<List<CourseData>>>
    {
        public Guid InstructorId { get; set; }
        public Guid EducationalYearId { get; set; } = Guid.Empty;
    }

    public class CourseData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
