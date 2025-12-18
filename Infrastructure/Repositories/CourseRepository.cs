using Application.DTOs.Course;
using Application.Features.Course.Query.GetAllCoursesForStudent;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CourseRepository(EducationDbContext context) : Repository<Course>(context), ICourseRepository
    {
        public async Task<IEnumerable<Course>> GetAllCoursesByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Where(c => c.InstructorCourses.Any(ic => ic.InstructorId == instructorId))
                .Include(c => c.CourseReviews)
                .Include(c => c.Sections)
                .Include(c => c.StudentCourses)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CourseByUserIdResponse>> GetAllCoursesByStudentIdAsync(GetAllCoursesEnrolledByStudentQuery req, CancellationToken cancellationToken = default)
        {
            return req.FirstThreeCoursesOnly ?
                await _context.Courses
                .Where(c => c.StudentCourses.Any(sc => sc.StudentId == req.StudentId))
                .Select(c => new CourseByUserIdResponse
                {
                    Id = c.Id,
                    Title = c.Name,
                    Price = c.Price ?? 0,

                    Rating = c.CourseReviews.Any()
                        ? c.CourseReviews.Average(r => r.StarRating)
                        : 0,

                    NumberOfStudents = c.StudentCourses.Count(),

                    NumberOfVideos = c.NumberOfVideos,

                    NumberOfSections = c.Sections.Count(),

                    NumberOfWatchedVideos = c.StudentCourses
                            .Where(sc => sc.StudentId == req.StudentId)
                            .Select(sc => sc.NumberOfCourseVideosWatched)
                            .FirstOrDefault(),

                    ThumbnailUrl = c.IntroVideoUrl ?? string.Empty,

                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .Take(3)
                .AsNoTracking()
                .ToListAsync(cancellationToken)
                :
             await _context.Courses
                .Where(c => c.StudentCourses.Any(sc => sc.StudentId == req.StudentId))
                .Select(c => new CourseByUserIdResponse
                {
                    Id = c.Id,
                    Title = c.Name,
                    Price = c.Price ?? 0,

                    Rating = c.CourseReviews.Any()
                        ? c.CourseReviews.Average(r => r.StarRating)
                        : 0,

                    NumberOfStudents = c.StudentCourses.Count(),

                    NumberOfVideos = c.NumberOfVideos,

                    NumberOfSections = c.Sections.Count(),

                    NumberOfWatchedVideos = c.StudentCourses
                            .Where(sc => sc.StudentId == req.StudentId)
                            .Select(sc => sc.NumberOfCourseVideosWatched)
                            .FirstOrDefault(),

                    ThumbnailUrl = c.IntroVideoUrl ?? string.Empty,

                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Course?> GetCourseDetailByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            // First, check if the course exists without loading all related entities 

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken: cancellationToken);
            if (course == null) return null;


            return await _context.Courses
                .Include(c => c.Sections)
                .Include(c => c.InstructorCourses)
                    .ThenInclude(ic => ic.Instructor)
                    .ThenInclude(i => i.User)
                .Include(c => c.StudentCourses)
                .Include(c => c.CourseReviews)
                    .ThenInclude(r => r.Student)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);
        }
    }
}
