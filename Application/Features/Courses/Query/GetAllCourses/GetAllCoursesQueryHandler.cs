using Application.Interfaces.BaseFilters;
using Application.ResultWrapper;
using Application.DTOs.Courses;
using Application.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Courses.Query.GetAllCourses
{
    public class GetAllCoursesQueryHandler(IUnitOfWork unitOfWork, IBaseFilterRegistry<Course> courseFilterRegistry)
        : IRequestHandler<GetAllCoursesQuery, Result<PaginatedResult<CourseResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<Course> _courseFilterRegistry = courseFilterRegistry;

        public async Task<Result<PaginatedResult<CourseResponse>>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var Courses = _unitOfWork.Repository<Course>().GetAll(cancellationToken);

                Courses = Courses.ApplyFilters(request.Filters, _courseFilterRegistry.Filters)
                                 .ApplySort(request.SortBy, request.IsDescending, _courseFilterRegistry.Sorts);

                var response = Courses.Select(course => new CourseResponse
                {
                    Id = course.Id,
                    Title = course.Name,
                    Price = course.Price ?? 0,
                    Description = course.Description ?? string.Empty,
                    PictureUrl = course.PictureUrl,
                    Rating = course.Rating,
                    IsEnrolled = course.StudentCourses.Any(sc => sc.StudentId == request.UserID),
                    NumberOfStudents = course.NumberOfStudentsEnrolled,
                    NumberOfVideos = course.NumberOfVideos,
                    NumberOfSections = course.NumberOfSections,

                    NumberOfWatchedVideos = course.StudentCourses
                        .Where(sc => sc.StudentId == request.UserID)
                        .Select(sc => sc.NumberOfCourseVideosWatched)
                        .FirstOrDefault(),

                    NumberOfSubscriptedSections = course.StudentCourses.Any(sc => sc.StudentId == request.UserID)
                        ? null
                        : course.Sections
                            .SelectMany(s => s.StudentSections)
                            .Where(ss => ss.StudentId == request.UserID)
                            .Select(ss => ss.SectionId)
                            .Distinct()
                            .Count(),

                    ProgressPercentage = course.StudentCourses.Any(sc => sc.StudentId == request.UserID) && course.NumberOfVideos > 0
                        ? ((decimal?)course.StudentCourses
                            .Where(sc => sc.StudentId == request.UserID)
                            .Select(sc => sc.NumberOfCourseVideosWatched)
                            .FirstOrDefault() / course.NumberOfVideos) * 100
                        : (decimal?)null,

                    ThumbnailUrl = course.IntroVideoUrl!,
                    CreatedAt = course.CreatedAt,
                    UpdatedAt = course.UpdatedAt,
                    NumberOfSheets = course.NumberOfQuestionSheets
                }).ToList();

                int pageSize = 10;
                int skip = (request.PageNumber - 1) * pageSize;
                var PaginatedResponse = response.Skip(skip).Take(pageSize).ToList();

                return Result<PaginatedResult<CourseResponse>>.Success(new PaginatedResult<CourseResponse>
                {
                    Items = PaginatedResponse,
                    PageNumber = request.PageNumber,
                    PageSize = pageSize,
                    TotalCount = response.Count
                });
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<PaginatedResult<CourseResponse>>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<CourseResponse>>.FailureStatusCode($"An error occurred while retrieving courses: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
