using Application.DTOs.Course;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Course.Query.GetAllCoursesForStudent
{
    public class GetAllCoursesEnrolledByStudentQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetAllCoursesEnrolledByStudentQuery, Result<PaginatedResult<CourseByUserIdResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<CourseByUserIdResponse>>> Handle(GetAllCoursesEnrolledByStudentQuery request, CancellationToken cancellationToken)
        {
            try
            {

                var courses = await _unitOfWork.GetRepository<ICourseRepository>()
                                               .GetAllCoursesByStudentIdAsync(request.StudentId, cancellationToken);

                var response = courses.Select(course => new CourseByUserIdResponse
                {
                    Id = course.Id,
                    Title = course.Title,
                    Price = course.Price,
                    Rating = course.Rating,
                    NumberOfStudents = course.NumberOfStudents,
                    NumberOfVideos = course.NumberOfVideos,
                    NumberOfSections = course.NumberOfSections,
                    NumberOfWatchedVideos = course.NumberOfWatchedVideos,
                    ThumbnailUrl = course.ThumbnailUrl,
                    CreatedAt = course.CreatedAt,
                    UpdatedAt = course.UpdatedAt
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
