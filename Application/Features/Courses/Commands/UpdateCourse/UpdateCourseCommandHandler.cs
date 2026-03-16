using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommandHandler(IUnitOfWork unitOfWork, ICloudinaryCore cloudinaryService) : IRequestHandler<UpdateCourseCommand, Result<string>>
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<string>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var course = await unitOfWork.Repository<Course>().GetByIdAsync(request.Id, cancellationToken)
                                                        ?? throw new KeyNotFoundException("Course not found");

                var Instrcutor = course.InstructorCourses.Where(c => c.CourseId == request.Id).FirstOrDefault()
                                                        ?? new InstructorCourse()
                                                        {
                                                            CourseId = request.Id,
                                                            InstructorId = request.InstructorId
                                                        };
                course.Name = request.CourseName;
                course.Description = request.Description;

                if (request.EducationYearId.HasValue)
                {
                    course.EducationYearId = request.EducationYearId.Value;
                }

                Instrcutor.InstructorId = request.InstructorId;

                if (request.PictureFile is not null)
                {
                    course.PictureUrl = await _cloudinaryService.UploadMediaAsync(request.PictureFile, UsageCategory.CourseThumbnail);
                }

                else if (!string.IsNullOrWhiteSpace(request.PictureUrl))
                {
                    course.PictureUrl = request.PictureUrl;
                }

                course.IntroVideoUrl = request.IntroVideoUrl;
                course.UpdatedAt = DateTime.UtcNow;

                if (course.StudentCourses.Count == 0 && request.Price.HasValue)
                {
                    course.Price = request.Price.Value;
                }

                unitOfWork.Repository<Course>().Update(course);
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
