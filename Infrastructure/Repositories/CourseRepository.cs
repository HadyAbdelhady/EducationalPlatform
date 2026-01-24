using Application.DTOs.Courses;
using Application.Features.Courses.Query.GetCourseById;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
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
                            IsEnrolled = course.StudentCourses.Count != 0,
                            UpdatedAt = course.UpdatedAt ?? course.CreatedAt,
                            Price = course.Price ?? 0,
                            IntroVideoUrl = course.IntroVideoUrl,
                            NumberOfVideos = course.NumberOfVideos,
                            NumberOfSheets = course.NumberOfQuestionSheets,
                            NumberOfSections = course.NumberOfSections,
                            NumberOfStudents = course.NumberOfStudentsEnrolled,
                            NumberOfEnrolledSections = course.StudentCourses.Count != 0
                                                                  ? 0
                                                                  : course.Sections
                                                                          .SelectMany(s => s.StudentSections)
                                                                          .Where(ss => ss.StudentId == request.UserId)
                                                                          .Distinct().Count(),

                            Rating = course.Rating,
                            NumberOfWatchedVideos = course.StudentCourses.Select(sc => sc.NumberOfCourseVideosWatched)
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
                        };


            var result = await query.FirstOrDefaultAsync(cancellationToken);
            if (result == null) return null;

            result.ProgressPercentage = result.NumberOfWatchedVideos > 0 && result.NumberOfVideos > 0
                                        ? result.NumberOfWatchedVideos / result.NumberOfVideos * 100
                                        : 0;

            return result;
        }


    }
}
