using Application.DTOs.Course;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Course.Query.GetAllCoursesForStudent
{
    public class GetAllCoursesEnrolledByStudentQuery : IRequest<Result<PaginatedResult<CourseByUserIdResponse>>>
    {
        public Guid StudentId { get; set; }
        public bool FirstThreeCoursesOnly { get; set; }
    }
}
