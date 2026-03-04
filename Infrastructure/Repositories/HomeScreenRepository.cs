using Application.DTOs.HomeScreen;
using Application.Interfaces;
using Domain.enums;
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
                                .OrderBy(sc => sc.Name)
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

        public async Task<InstructorDashboardResponse?> GetInstructorDashboardDataAsync(
            Guid instructorId,
            CancellationToken cancellationToken = default)
        {
            // Get instructor's courses first
            var instructorCourseIds = _context.InstructorCourses
                .Where(ic => ic.InstructorId == instructorId)
                .Select(ic => ic.CourseId)
                .ToList();

            InstructorDashboardResponse? response = new()
            {
                // Instructor's courses with metrics
                Courses = _context.InstructorCourses
                    .Where(ic => ic.InstructorId == instructorId)
                    .Select(ic => new InstructorCourseDto
                    {
                        Id = ic.Course.Id,
                        Name = ic.Course.Name,
                        PictureUrl = ic.Course.PictureUrl,
                        NumberOfStudents = ic.Course.NumberOfStudentsEnrolled,
                        Rating = ic.Course.Rating,
                        Revenue = _context.Payments
                            .Where(p => p.CourseId == ic.CourseId && p.Status == PaymentStatus.Completed)
                            .Sum(p => p.Amount),
                        CreatedAt = ic.Course.CreatedAt,
                        NumberOfVideos = ic.Course.NumberOfVideos,
                        NumberOfExams = ic.Course.NumberOfExams,
                        NumberOfSheets = ic.Course.NumberOfQuestionSheets
                    })
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(6)
                    .ToList(),

                // Instructor statistics
                Stats = new List<InstructorStatsDto>
                {
                    new()
                    {
                        TotalCourses = instructorCourseIds.Count,
                        TotalStudents = _context.InstructorCourses
                            .Where(ic => ic.InstructorId == instructorId)
                            .Sum(ic => ic.Course.NumberOfStudentsEnrolled),
                        TotalRevenue = _context.InstructorCourses
                            .Where(ic => ic.InstructorId == instructorId)
                            .SelectMany(ic => ic.Course.Payments)
                            .Where(p => p.Status == PaymentStatus.Completed)
                            .Sum(p => p.Amount),
                        AverageRating = _context.InstructorCourses
                            .Where(ic => ic.InstructorId == instructorId)
                            .Average(ic => ic.Course.Rating ?? 0),
                        TotalVideos = _context.InstructorCourses
                            .Where(ic => ic.InstructorId == instructorId)
                            .Sum(ic => ic.Course.NumberOfVideos),
                        TotalExams = _context.InstructorCourses
                            .Where(ic => ic.InstructorId == instructorId)
                            .Sum(ic => ic.Course.NumberOfExams),
                        TotalSheets = _context.InstructorCourses
                            .Where(ic => ic.InstructorId == instructorId)
                            .Sum(ic => ic.Course.NumberOfQuestionSheets)
                    }
                }
            };

            // Recent activities
            var videos = _context.Videos
                .Where(v => instructorCourseIds.Contains(v.Section!.CourseId))
                .Select(v => new RecentActivityDto
                {
                    ActivityType = "Video",
                    Description = v.Name,
                    CourseName = v.Section!.Course!.Name,
                    Timestamp = v.CreatedAt,
                    RelatedEntityId = v.Id
                });

            var exams = _context.Exams
                .Where(e => e.InstructorId == instructorId)
                .Select(e => new RecentActivityDto
                {
                    ActivityType = "Exam",
                    Description = e.Name,
                    CourseName = e.Course!.Name,
                    Timestamp = e.CreatedAt,
                    RelatedEntityId = e.Id
                });

            var sheets = _context.Sheets
                .Where(s => s.InstructorId == instructorId)
                .Select(s => new RecentActivityDto
                {
                    ActivityType = "Sheet",
                    Description = s.Name,
                    CourseName = s.Course!.Name,
                    Timestamp = s.CreatedAt,
                    RelatedEntityId = s.Id
                });

            var enrollments = _context.StudentCourses
                .Where(sc => instructorCourseIds.Contains(sc.CourseId))
                .Select(sc => new RecentActivityDto
                {
                    ActivityType = "Enrollment",
                    Description = "New student enrolled",
                    CourseName = sc.Course.Name,
                    Timestamp = sc.EnrolledAt,
                    RelatedEntityId = null
                });

            response.RecentActivities = videos.Concat(exams).Concat(sheets).Concat(enrollments)
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .ToList();

            var sheetsToReview = _context.Sheets
                .Where(s => s.InstructorId == instructorId && s.DueDate.HasValue &&
                           s.DueDate.Value <= DateTimeOffset.Now.AddDays(3))
                .Select(s => new PendingTaskDto
                {
                    TaskType = "Review",
                    Title = $"Review sheet: {s.Name}",
                    CourseName = s.Course!.Name,
                    DueDate = s.DueDate!.Value,
                    Priority = 2,
                    RelatedEntityId = s.Id
                });

            var supportTasks = _context.StudentCourses
                .Where(sc => instructorCourseIds.Contains(sc.CourseId) && sc.EnrolledAt >= DateTimeOffset.Now.AddDays(-7))
                .Select(sc => new PendingTaskDto
                {
                    TaskType = "Support",
                    Title = "Answer student questions",
                    CourseName = sc.Course.Name,
                    DueDate = null,
                    Priority = 3,
                    RelatedEntityId = null
                });

            response.PendingTasks = sheetsToReview.Concat(supportTasks)
                .OrderBy(t => t.Priority)
                .ThenBy(t => t.DueDate ?? DateTimeOffset.MaxValue)
                .Take(8)
                .ToList();

            // Upcoming exams
            response.UpcomingExams = _context.Exams
                .Where(e => e.InstructorId == instructorId && e.StartTime.HasValue && e.StartTime.Value >= DateTimeOffset.UtcNow)
                .OrderBy(e => e.StartTime)
                .Select(e => new UpcomingExamDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    CourseName = e.Course!.Name,
                    StartTime = e.StartTime!.Value,
                    DurationInMinutes = e.DurationInMinutes ?? 0,
                    NumberOfQuestions = e.NumberOfQuestions,
                    Status = e.Status.ToString(),
                    NumberOfEnrolledStudents = _context.StudentCourses.Count(sc => sc.CourseId == e.CourseId)
                })
                .Take(5)
                .ToList();

            // Upcoming sheets
            response.UpcomingSheets = _context.Sheets
                .Where(s => s.InstructorId == instructorId && s.DueDate.HasValue && s.DueDate.Value >= DateTimeOffset.UtcNow.AddDays(-7))
                .OrderBy(s => s.DueDate)
                .Select(s => new UpcomingSheetDto
                {
                    Id = s.Id,
                    Title = s.Name,
                    CourseName = s.Course!.Name,
                    DueDate = s.DueDate,
                    Status = "Published",
                    NumberOfSubmittedStudents = s.AnswersSheets.Count
                })
                .Take(5)
                .ToList();

            return response;
        }
    }
}
