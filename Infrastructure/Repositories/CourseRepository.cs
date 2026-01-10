using Application.DTOs.Courses;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CourseRepository(EducationDbContext context) : Repository<Course>(context), ICourseRepository
    {
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
                            //Sections = course.Sections.Select(s => new SectionDetailDto
                            //{
                            //    Id = s.Id,
                            //    Name = s.Name,
                            //    Description = s.Description,
                            //    NumberOfVideos = s.NumberOfVideos,
                            //    Rating = s.Rating,
                            //    Price = s.Price
                            //}).ToList(),

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
                            //Reviews = course.CourseReviews
                            //    .Where(r => r.Student != null && r.Student.User != null)
                            //    .OrderByDescending(r => r.CreatedAt)
                            //    .Select(r => new CourseReviewDto
                            //    {
                            //        Id = r.Id,
                            //        StarRating = r.StarRating,
                            //        Comment = r.Comment,
                            //        CreatedAt = r.CreatedAt,
                            //        Student = new StudentReviewInfoDto
                            //        {
                            //            StudentId = r.StudentId,
                            //            FullName = r.Student.User.FullName,
                            //            PersonalPictureUrl = r.Student.User.PersonalPictureUrl
                            //        }
                            //    })
                            //    .Take(3)
                            //    .ToList()
                        };

            var result = await query.FirstOrDefaultAsync(cancellationToken);
            if (result == null) return null;

            // Calculate rating and counts in-memory (minimal overhead)
            //result.TotalReviews = result.Reviews.Count;

            return result;
        }

       
    }
}
