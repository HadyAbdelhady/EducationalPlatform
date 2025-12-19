using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Courses.Commands.DeleteCourse
{
    public class DeleteCourseCommand : IRequest<Result<string>>
    {
        public Guid CourseId { get; set; }
    }
}
