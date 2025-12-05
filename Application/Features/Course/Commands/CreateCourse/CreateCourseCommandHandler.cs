using Application.DTOs.Course;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Course.Commands.CreateCourse
{
    public class CreateCourseCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCourseCommand, Result<CourseCreationResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<CourseCreationResponse>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
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

                return Result<CourseCreationResponse>.Success(new CourseCreationResponse
                {
                    CourseId = newCourse.Id,
                    CourseName = newCourse.Name,
                    CreatedAt = newCourse.CreatedAt.DateTime
                });
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<CourseCreationResponse>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<CourseCreationResponse>.FailureStatusCode($"Error creating course: {ex.Message}", ErrorType.Conflict);
            }
        }

    }
}
