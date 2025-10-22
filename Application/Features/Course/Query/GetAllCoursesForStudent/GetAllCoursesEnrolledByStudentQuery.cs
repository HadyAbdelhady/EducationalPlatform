using Application.DTOs.Course;
using MediatR;

namespace Application.Features.Course.Query.GetAllCoursesForStudent
{
    public class GetAllCoursesEnrolledByStudentQuery : IRequest<List<CourseByUserIdResponse>>
    {
        public Guid StudentId { get; set; }
    }
}
