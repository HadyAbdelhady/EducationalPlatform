using Application.DTOs.Courses;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Courses.Query.GetAllCoursesByInstructor
{
    public class GetAllCoursesByInstructorQueryHandler(
                                                        IUnitOfWork unitOfWork
                                                      ) : IRequestHandler<GetAllCoursesByInstructorQuery,
                                                          Result<PaginatedResult<CourseByUserIdResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<CourseByUserIdResponse>>> Handle(GetAllCoursesByInstructorQuery request, CancellationToken cancellationToken)
        {
            try
            {

                var courses = await _unitOfWork.GetRepository<ICourseRepository>()
                                                               .GetAllCoursesByInstructorIdAsync(request.InstructorId, cancellationToken);

                var response = courses.Select(course => new CourseByUserIdResponse
                {
                    Id = course.Id,
                    Title = course.Name,
                    Price = course.Price ?? 0,
                    Rating = course.Rating,
                    NumberOfStudents = course.StudentCourses?.Count ?? 0,
                    NumberOfVideos = course.NumberOfVideos,
                    NumberOfSections = course.Sections?.Count ?? 0,
                    ThumbnailUrl = course.IntroVideoUrl ?? string.Empty,
                    CreatedAt = course.CreatedAt,
                    NumberOfSheets = course.NumberOfQuestionSheets,
                    UpdatedAt = course.UpdatedAt ?? course.CreatedAt
                }).ToList();

                return Result<PaginatedResult<CourseByUserIdResponse>>.Success(new PaginatedResult<CourseByUserIdResponse>
                {
                    Items = response,
                    PageNumber = 1,
                    PageSize = response.Count,
                    TotalCount = response.Count
                });
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<PaginatedResult<CourseByUserIdResponse>>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<CourseByUserIdResponse>>.FailureStatusCode($"An error occurred while retrieving courses: {ex.Message}", ErrorType.InternalServerError);
            }
        }

    }
}
