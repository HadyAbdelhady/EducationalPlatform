using Application.Interfaces;
using MediatR;

namespace Application.Features.Course.Commands.DeleteCourse
{
    public class DeleteCourseCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCourseCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<string> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId)
                                                        ?? throw new KeyNotFoundException("Course not found");

            await _unitOfWork.Courses.RemoveAsync(course.Id,cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return "Course deleted successfully";
        }
    }
}
