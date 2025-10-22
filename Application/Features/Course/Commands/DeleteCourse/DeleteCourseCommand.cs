using MediatR;

namespace Application.Features.Course.Commands.DeleteCourse
{
    public class DeleteCourseCommand : IRequest<string>
    {
        public Guid CourseId { get; set; }
    }
}
