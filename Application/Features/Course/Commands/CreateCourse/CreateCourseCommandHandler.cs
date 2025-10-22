using Application.DTOs.Course;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Course.Commands.CreateCourse
{
    public class CreateCourseCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCourseCommand, CourseCreationResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<CourseCreationResponse> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Course newCourse = new()
            {
                Id = Guid.NewGuid(),
                Name = request.CourseName,
                Description = request.Description,
                Price = request.Price,
                PictureUrl = request.PictureUrl,
                IntroVideoUrl = request.IntroVideoUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,

            };

            await _unitOfWork.Courses.AddAsync(newCourse, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            newCourse.InstructorCourses.Add(new Domain.Entities.InstructorCourse
            {
                InstructorId = request.InstructorId,
                CourseId = newCourse.Id
            });

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CourseCreationResponse
            {
                CourseId = newCourse.Id,
                CourseName = newCourse.Name,
                CreatedAt = newCourse.CreatedAt.DateTime
            };

        }

    }
}
