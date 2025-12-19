using Application.DTOs.Course;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Course.Query.GetAllCourses
{
    public class GetAllCoursesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllCoursesQuery, Result<PaginatedResult<CourseByUserIdResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<CourseByUserIdResponse>>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var courses = await _unitOfWork.GetRepository<ICourseRepository>().GetAllAsync(cancellationToken);

                var response = courses.Select(course => new CourseByUserIdResponse
                {
                    Id = course.Id,
                    Title = course.Name,
                    Price = (decimal)course.Price!,
                    Rating = course.CourseReviews.Count > 0 ? course.CourseReviews.Average(r => r.StarRating) : 0,
                    NumberOfStudents = course.StudentCourses?.Count ?? 0,
                    NumberOfVideos = course.NumberOfVideos,
                    NumberOfSections = course.Sections?.Count ?? 0,
                    ThumbnailUrl = course.IntroVideoUrl!,
                    CreatedAt = course.CreatedAt,
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
