using Application.Interfaces.BaseFilters;
using Application.ResultWrapper;
using Application.DTOs.Courses;
using Application.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Application.HelperFunctions;

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

                Courses = Courses.ApplyFilters(request.GetAllEntityRequestSkeleton.Filters, _courseFilterRegistry.Filters)
                                 .ApplySort(request.GetAllEntityRequestSkeleton.SortBy, request.GetAllEntityRequestSkeleton.IsDescending, _courseFilterRegistry.Sorts);

                var response = Courses
                .Select(course => new
                {
                    course,

                    StudentCourse = course.StudentCourses.Where(sc => sc.StudentId == request.UserID && sc.CourseId == course.Id),

                    SubscribedSections = course.Sections.SelectMany(s => s.StudentSections)
                                                        .Where(ss => ss.StudentId == request.UserID)
                })
                .Select(x => new CourseResponse
                {
                    Id = x.course.Id,
                    Title = x.course.Name,
                    Description = x.course.Description ?? string.Empty,
                    PictureUrl = x.course.PictureUrl,
                    Price = x.course.Price ?? 0,
                    Rating = x.course.Rating,

                    IsEnrolled = x.StudentCourse.Any(),

                    NumberOfStudents = x.course.NumberOfStudentsEnrolled,
                    NumberOfVideos = x.course.NumberOfVideos,
                    NumberOfSections = x.course.NumberOfSections,
                    NumberOfSheets = x.course.NumberOfQuestionSheets,

                    NumberOfWatchedVideos = x.StudentCourse.Any()
                                                            ? x.StudentCourse.Select(xx => xx.NumberOfCourseVideosWatched)
                                                                                .FirstOrDefault()
                                                            : 0,

                    NumberOfSubscriptedSections = x.StudentCourse.Any()
                                                                  ? 0
                                                                  : x.SubscribedSections.Distinct().Count(),

                    ProgressPercentage = x.StudentCourse.Any()
                                                             ? x.StudentCourse.Select(xx => xx.Progress).FirstOrDefault()
                                                             : x.SubscribedSections.Select(ss => ss.Progress).Average(),

                    ThumbnailUrl = x.course.IntroVideoUrl!,
                    CreatedAt = x.course.CreatedAt,
                    UpdatedAt = x.course.UpdatedAt
                }).ToList();


                int pageSize = 10;
                int skip = (request.GetAllEntityRequestSkeleton.PageNumber - 1) * pageSize;
                var PaginatedResponse = response.Skip(skip).Take(pageSize).ToList();

                return Result<PaginatedResult<CourseResponse>>.Success(new PaginatedResult<CourseResponse>
                {
                    Items = PaginatedResponse,
                    PageNumber = request.GetAllEntityRequestSkeleton.PageNumber,
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
