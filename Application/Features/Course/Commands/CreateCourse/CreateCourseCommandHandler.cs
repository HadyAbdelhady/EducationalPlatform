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
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
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

                await _unitOfWork.Repository<Domain.Entities.Course>().AddAsync(newCourse, cancellationToken);

                newCourse.InstructorCourses.Add(new Domain.Entities.InstructorCourse
                {
                    InstructorId = request.InstructorId,
                    CourseId = newCourse.Id,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return new CourseCreationResponse
                {
                    CourseId = newCourse.Id,
                    CourseName = newCourse.Name,
                    CreatedAt = newCourse.CreatedAt.DateTime
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

    }
}
