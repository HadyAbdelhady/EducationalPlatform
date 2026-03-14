using Application.DTOs.Courses;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandHandler(IUnitOfWork unitOfWork, ICloudinaryCore cloudinaryService) : IRequestHandler<CreateCourseCommand, Result<CourseCreationResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<CourseCreationResponse>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string? pictureUrl = request.PictureUrl;

                if (request.PictureFile is not null)
                {
                    pictureUrl = await _cloudinaryService.UploadMediaAsync(request.PictureFile, UsageCategory.CourseThumbnail);
                }

                Course newCourse = new()
                {
                    Id = Guid.NewGuid(),
                    Name = request.CourseName,
                    Description = request.Description,
                    EducationYearId = request.EducationYearId,
                    Price = request.Price,
                    PictureUrl = pictureUrl,
                    IntroVideoUrl = request.IntroVideoUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _unitOfWork.Repository<Course>().AddAsync(newCourse, cancellationToken);

                newCourse.InstructorCourses.Add(new InstructorCourse
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
                    EducationYearId = newCourse.EducationYearId,
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
