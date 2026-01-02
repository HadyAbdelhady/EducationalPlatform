using Application.DTOs.Courses;
using Application.Features.Courses.Query.GetAllCoursesForStudent;
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
            return
             await _context.Courses
                .Where(c => c.StudentCourses.Any(sc => sc.StudentId == req.StudentId))
                .Select(c => new CourseByUserIdResponse
                {
                    Id = c.Id,
                    Title = c.Name,
                    Price = c.Price ?? 0,

                    Rating = c.Rating,

                    NumberOfStudents = c.NumberOfStudentsEnrolled,

                    NumberOfVideos = c.NumberOfVideos,

                    NumberOfSections = c.NumberOfSections,

                    NumberOfWatchedVideos = c.StudentCourses
                            .Where(sc => sc.StudentId == req.StudentId)
                            .Select(sc => sc.NumberOfCourseVideosWatched)
                            .FirstOrDefault(),

                    ThumbnailUrl = c.IntroVideoUrl ?? string.Empty,

                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt ?? c.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<CourseDetailResponse?> GetCourseDetailResponseByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            var query = from course in _context.Courses
                        where course.Id == courseId
                        select new CourseDetailResponse
                        {
                            Id = course.Id,
                            Title = course.Name,
                            Description = course.Description,
                            PictureUrl = course.PictureUrl,
                            CreatedAt = course.CreatedAt,
                            UpdatedAt = course.UpdatedAt ?? course.CreatedAt,
                            Price = course.Price ?? 0,
                            IntroVideoUrl = course.IntroVideoUrl,
                            NumberOfVideos = course.NumberOfVideos,
                            NumberOfSheets = course.NumberOfQuestionSheets,
                            NumberOfSections = course.NumberOfSections,
                            NumberOfStudents = course.NumberOfStudentsEnrolled,
                            Rating = course.Rating,
                            // Sections
                            Sections = course.Sections.Select(s => new SectionDetailDto
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Description = s.Description,
                                NumberOfVideos = s.NumberOfVideos,
                                Rating = s.Rating,
                                Price = s.Price
                            })
                            //.Take(3)
                            .ToList(),

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

                            // Reviews + Student info
                            Reviews = course.CourseReviews
                                .Where(r => r.Student != null && r.Student.User != null)
                                .OrderByDescending(r => r.CreatedAt)
                                .Select(r => new CourseReviewDto
                                {
                                    Id = r.Id,
                                    StarRating = r.StarRating,
                                    Comment = r.Comment,
                                    CreatedAt = r.CreatedAt,
                                    Student = new StudentReviewInfoDto
                                    {
                                        StudentId = r.StudentId,
                                        FullName = r.Student.User.FullName,
                                        PersonalPictureUrl = r.Student.User.PersonalPictureUrl
                                    }
                                })
                                .Take(3)
                                .ToList()
                        };

            var result = await query.FirstOrDefaultAsync(cancellationToken);
            if (result == null) return null;

            // Calculate rating and counts in-memory (minimal overhead)
            result.TotalReviews = result.Reviews.Count;

            return result;
        }
    }
}
