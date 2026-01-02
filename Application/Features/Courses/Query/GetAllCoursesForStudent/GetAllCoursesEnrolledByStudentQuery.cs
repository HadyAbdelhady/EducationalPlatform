using Application.DTOs.Courses;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Courses.Query.GetAllCoursesForStudent
{
    public class GetAllCoursesEnrolledByStudentQuery : IRequest<Result<PaginatedResult<CourseByUserIdResponse>>>
    {
        public Guid StudentId { get; set; }
        //public bool FirstThreeCoursesOnly { get; set; }
    }
}
