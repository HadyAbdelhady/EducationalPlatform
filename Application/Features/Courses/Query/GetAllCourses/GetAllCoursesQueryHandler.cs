using Application.DTOs.Courses;
using Application.Interfaces;
using Application.Interfaces.BaseFilters;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Courses.Query.GetAllCourses
{
    public class GetAllCoursesQueryHandler(IUnitOfWork unitOfWork, IBaseFilterRegistry<Course> courseFilterRegistry) : IRequestHandler<GetAllCoursesQuery, Result<PaginatedResult<CourseResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<Course> _courseFilterRegistry = courseFilterRegistry;

        public async Task<Result<PaginatedResult<CourseResponse>>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var Courses = _unitOfWork.Repository<Course>().GetAll(cancellationToken);

                Courses = _courseFilterRegistry.ApplyFilter(Courses, request.Filters);
                Courses = _courseFilterRegistry.ApplySort(Courses, request.SortBy, request.IsDescending);

                var response = Courses.Select(course => new CourseResponse
                {
                    Id = course.Id,
                    Title = course.Name,
                    Price = course.Price ?? 0,
                    Rating = course.Rating,
                    NumberOfStudents = course.StudentCourses.Count,
                    NumberOfVideos = course.NumberOfVideos,
                    NumberOfSections = course.Sections.Count,
                    ThumbnailUrl = course.IntroVideoUrl!,
                    CreatedAt = course.CreatedAt,
                    NumberOfSheets = course.NumberOfQuestionSheets,
                    UpdatedAt = course.UpdatedAt ?? course.CreatedAt
                }).ToList();

                // Pagination
                int skip = (request.PageNumber - 1) * 10;
                var PagenatedResponse = response.Skip(skip).Take(10).ToList();

                return Result<PaginatedResult<CourseResponse>>.Success(new PaginatedResult<CourseResponse>
                {
                    Items = PagenatedResponse,
                    PageNumber = request.PageNumber,
                    PageSize = 10,
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
