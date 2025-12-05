using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Course.Commands.UpdateCourse
{
    public class UpdateCourseCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateCourseCommand, Result<string>>
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public async Task<Result<string>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var course = await unitOfWork.Repository<Domain.Entities.Course>().GetByIdAsync(request.Id, cancellationToken)
                                                        ?? throw new KeyNotFoundException("Course not found");

                var Instrcutor = course.InstructorCourses.Where(c => c.CourseId == request.Id).FirstOrDefault()
                                                        ?? new InstructorCourse()
                                                        {
                                                            CourseId = request.Id,
                                                            InstructorId = request.InstructorId
                                                        };
                course.Name = request.CourseName;
                course.Description = request.Description;
                Instrcutor.InstructorId = request.InstructorId;
                course.Price = request.Price;
                course.PictureUrl = request.PictureUrl;
                course.IntroVideoUrl = request.IntroVideoUrl;
                course.UpdatedAt = DateTime.UtcNow;
                unitOfWork.Repository<Domain.Entities.Course>().Update(course);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<string>.Success("Course updated successfully");
            }
            catch (KeyNotFoundException)
            {
                return Result<string>.FailureStatusCode("Course not found", ErrorType.NotFound);
            }
            catch (Exception ex)
            {
                return Result<string>.FailureStatusCode($"An error occurred while updating the course: {ex.Message}", ErrorType.Validation);
            }
        }
    }
}
