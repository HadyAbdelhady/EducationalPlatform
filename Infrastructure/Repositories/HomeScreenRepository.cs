using Application.DTOs.HomeScreen;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class HomeScreenRepository(EducationDbContext context) : IHomeScreenRepository
    {
        private readonly EducationDbContext _context = context;

        public async Task<StudentHomeScreenResponse?> GetStudentHomeScreenDataAsync(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            var query = from sc in _context.StudentCourses
                        where sc.StudentId == studentId
                        select new StudentHomeScreenResponse
                        {
                            // Enrolled courses - using navigation properties like GetCourseDetailResponseByIdAsync
                            Courses = _context.StudentCourses
                                .Where(sc2 => sc2.StudentId == studentId)
                                .Select(sc2 => new EnrolledCourseDto
                                {
                                    Id = sc2.Course.Id,
                                    Name = sc2.Course.Name,
                                    PictureUrl = sc2.Course.PictureUrl,
                                    NumberOfVideos = sc2.Course.NumberOfVideos,
                                    NumberOfExams = sc2.Course.NumberOfExams,
                                    NumberOfSheets = sc2.Course.NumberOfQuestionSheets,

                                    Rating = sc2.Course.Rating
                                })
                                .OrderBy(sc=> sc.Name)
                                .Take(6)

                                .ToList(),

                            // Latest videos
                            Videos = _context.Videos
                                .OrderByDescending(v => v.CreatedAt)
                                .Select(v => new LatestVideoDto
                                {
                                    Id = v.Id,
                                    Name = v.Name,
                                    CreatedAt = v.CreatedAt
                                })
                                .Take(3)
                                .OrderBy(v => v.CreatedAt)
                                .ToList(),

                            // Exams from enrolled courses - using subquery pattern
                            Exams = _context.Exams
                                .Where(e => e.StartTime.HasValue &&
                                           _context.StudentCourses
                                               .Any(sc3 => sc3.StudentId == studentId &&
                                                        sc3.CourseId == e.CourseId))
                                .Select(e => new StudentExamDto
                                {
                                    Id = e.Id,
                                    Title = e.Name,
                                    CourseName = e.Course != null ? e.Course.Name : string.Empty,
                                    TotalMark = e.TotalMark,
                                    StartTime = e.StartTime,
                                    DurationInMinutes = e.DurationInMinutes
                                })
                                .OrderBy(e => e.StartTime)
                                .Take(3)
                                .ToList(),

                            // Sheets from enrolled courses - using subquery pattern
                            Sheets = _context.Sheets
                                .Where(s => s.CourseId.HasValue &&
                                           s.DueDate.HasValue &&
                                           _context.StudentCourses
                                               .Any(sc4 => sc4.StudentId == studentId &&
                                                        sc4.CourseId == s.CourseId.Value))
                                .Select(s => new StudentSheetDto
                                {
                                    Id = s.Id,
                                    Title = s.Name,
                                    CourseName = s.Course != null ? s.Course.Name : string.Empty,
                                    DueDate = s.DueDate
                                })
                                .OrderBy(s => s.DueDate)
                                .Take(3)
                                .ToList()
                        };

            var result = await query.FirstOrDefaultAsync(cancellationToken);

            return result;
        }
    }
}

