using Infrastructure.Common;
using Application.Features.Courses.DTOs;
using Application.Features.Courses.Query.GetCourseById;
using Application.Common.Interfaces;
using Application.Features.Courses.Interfaces;
using Domain.Entities;
using Infrastructure.Common.Data;
using Microsoft.EntityFrameworkCore;
using Application.Common;
using Application.Features.Courses.Query.GetAllCourses;
using Application.Features.EducationYears.Interfaces;

namespace Infrastructure.Features.Courses
{
    public class CourseRepository(EducationDbContext context) : Repository<Course>(context), ICourseRepository
    {
        public async Task<CourseDetailResponse?> GetCourseDetailResponseByIdAsync(GetCourseByIdQuery request, CancellationToken cancellationToken = default)
        {
            var query = from course in _context.Courses
                        where course.Id == request.CourseId
                        select new CourseDetailResponse
                        {
                            Id = course.Id,
                            Title = course.Name,
                            Description = course.Description,
                            PictureUrl = course.PictureUrl,
                            CreatedAt = course.CreatedAt,
                            IsEnrolled = course.StudentCourses.Any(sc => sc.StudentId == request.UserId)
                                || (course.Sections.Any()
                                    && course.Sections.All(s => s.StudentSections.Any(ss => ss.StudentId == request.UserId))),
                            UpdatedAt = course.UpdatedAt ?? course.CreatedAt,
                            Price = course.Price ?? 0,
                            IntroVideoUrl = course.IntroVideoUrl,
                            NumberOfVideos = course.NumberOfVideos,
                            NumberOfSheets = course.NumberOfQuestionSheets,
                            NumberOfSections = course.NumberOfSections,
                            NumberOfStudents = course.NumberOfStudentsEnrolled,
                            NumberOfEnrolledSections = (course.StudentCourses.Any(sc => sc.StudentId == request.UserId)
                                || (course.Sections.Any()
                                    && course.Sections.All(s => s.StudentSections.Any(ss => ss.StudentId == request.UserId))))
                                                                  ? 0
                                                                  : course.Sections
                                                                          .SelectMany(s => s.StudentSections)
                                                                          .Where(ss => ss.StudentId == request.UserId)
                                                                          .Distinct().Count(),

                            Rating = course.Rating,
                            NumberOfWatchedVideos = course.StudentCourses
                                .Where(sc => sc.StudentId == request.UserId)
                                .Select(sc => sc.NumberOfCourseVideosWatched)
                                .FirstOrDefault(),

                            // Instructors
                            Instructors = course.InstructorCourses
                                .Where(ic => ic.Instructor != null && ic.Instructor.User != null)
                                .Select(ic => new InstructorInfoDto
                                {
                                    InstructorId = ic.InstructorId,
                                    FullName = ic.Instructor.User.FullName,
                                    PersonalPictureUrl = ic.Instructor.User.PersonalPictureUrl,
                                    GmailExternal = ic.Instructor.User.GmailExternal
                                }).ToList(),

                            EducationYear = course.EducationYear != null ? course.EducationYear.EducationYearName : string.Empty,
                            EducationYearId = course.EducationYear != null ? course.EducationYear.Id : Guid.Empty

                        };


            var result = await query.FirstOrDefaultAsync(cancellationToken);
            if (result == null) return null;

            result.ProgressPercentage = result.NumberOfWatchedVideos > 0 && result.NumberOfVideos > 0
                                        ? result.NumberOfWatchedVideos / result.NumberOfVideos * 100
                                        : 0;

            return result;
        }

        public async Task<PaginatedResult<CourseResponse>> GetCoursesPaginatedAsync(
            GetAllCoursesQuery request,
            IBaseFilterRegistry<Course> courseFilterRegistry,
            IStudentEducationYearProvider studentEducationYearProvider,
            CancellationToken cancellationToken = default)
        {
            var Courses = _context.Courses
                                  .AsNoTracking()
                                  .ApplyFilters(request.GetAllEntityRequestSkeleton.Filters, courseFilterRegistry.Filters)
                                  .ApplySort(request.GetAllEntityRequestSkeleton.SortBy, request.GetAllEntityRequestSkeleton.IsDescending, courseFilterRegistry.Sorts);

            // For students: filter by their education year. Instructors/admins use Filters["educationyearid"] from request.
            var studentEducationYearId = await studentEducationYearProvider.GetEducationYearIdByUserIdAsync(request.UserID, cancellationToken);
            if (studentEducationYearId.HasValue)
            {
                Courses = Courses.Where(c => c.EducationYearId == studentEducationYearId.Value);
            }

            var userId = request.UserID;
            var coursesQuery = Courses
                .Select(course => new
                {
                    course,

                    StudentCourse = course.StudentCourses.Where(sc => sc.StudentId == userId && sc.CourseId == course.Id),

                    SubscribedSections = course.Sections.SelectMany(s => s.StudentSections)
                                                        .Where(ss => ss.StudentId == userId),

                    OwnsAllCurrentSections = course.Sections.Any()
                        && course.Sections.All(s => s.StudentSections.Any(ss => ss.StudentId == userId))
                })
                .Select(x => new CourseResponse
                {
                    Id = x.course.Id,
                    EducationYearName = x.course.EducationYear != null ? x.course.EducationYear.EducationYearName : string.Empty,
                    Title = x.course.Name,
                    Description = x.course.Description ?? string.Empty,
                    PictureUrl = x.course.PictureUrl,
                    Price = x.course.Price ?? 0,
                    Rating = x.course.Rating,

                    IsEnrolled = x.StudentCourse.Any() || x.OwnsAllCurrentSections,

                    NumberOfStudents = x.course.NumberOfStudentsEnrolled,
                    NumberOfVideos = x.course.NumberOfVideos,
                    NumberOfSections = x.course.NumberOfSections,
                    NumberOfSheets = x.course.NumberOfQuestionSheets,

                    NumberOfWatchedVideos = x.StudentCourse.Any()
                                                            ? x.StudentCourse.Select(xx => xx.NumberOfCourseVideosWatched)
                                                                                .FirstOrDefault()
                                                            : 0,

                    NumberOfSubscriptedSections = (x.StudentCourse.Any() || x.OwnsAllCurrentSections)
                        ? 0
                        : x.SubscribedSections.Distinct().Count(),

                    ProgressPercentage = x.StudentCourse.Any()
                                                             ? x.StudentCourse.Select(xx => xx.Progress).FirstOrDefault()
                                                             : x.SubscribedSections.Select(ss => ss.Progress).Average(),

                    ThumbnailUrl = x.course.IntroVideoUrl!,
                    CreatedAt = x.course.CreatedAt,
                    UpdatedAt = x.course.UpdatedAt
                });

            return await coursesQuery.ToPaginatedResultAsync(
                request.GetAllEntityRequestSkeleton.PageNumber,
                10,
                cancellationToken);
        }
    }
}
