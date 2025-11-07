using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Course.Commands.DeleteCourse
{
    public class DeleteCourseCommand : IRequest<Result<string>>
    {
        public Guid CourseId { get; set; }
    }
}
