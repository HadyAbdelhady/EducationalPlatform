using Application.Features.HomeScreen.DTOs;
using Application.Common.Interfaces;
using Application.Features.HomeScreen.Interfaces;
using Domain;
using Domain.enums;
using Infrastructure.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.HomeScreen
{
    public class HomeScreenRepository(EducationDbContext context) : IHomeScreenRepository
    {
        private readonly EducationDbContext _context = context;

        public async Task<StudentHomeScreenResponse?> GetStudentHomeScreenDataAsync(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            var studentEducationYearId = await _context.Students
                .AsNoTracking()
                .Where(s => s.UserId == studentId)
                .Select(s => s.EducationYearId)
                .FirstOrDefaultAsync(cancellationToken);

            var query = from student in _context.Students
                        where student.UserId == studentId
                        select new StudentHomeScreenResponse
                        {
                            // Enrolled courses - scope to student's education year for consistency
                            Courses = _context.StudentCourses
                                .Where(sc2 => sc2.StudentId == studentId && sc2.Course.EducationYearId == studentEducationYearId)
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

                            // Latest videos - scope to courses in student's education year
                            Videos = _context.Videos
                                .Where(v => v.Section != null 
                                        && v.Section.Course.EducationYearId == studentEducationYearId 
                                        && (v.Section.StudentSections.Any(ss => ss.StudentId == studentId)
                                            || v.Section.Course.StudentCourses.Any(sc => sc.StudentId == studentId)))
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

                            // Exams from enrolled courses - scope to student's education year
                            Exams = _context.Exams
                                .Where(e => e.StartTime.HasValue &&
                                           e.EndTime > EgyptTime.UtcNow &&
                                           e.Course.EducationYearId == studentEducationYearId &&
                                           (_context.StudentCourses
                                               .Any(sc3 => sc3.StudentId == studentId &&
                                                        sc3.CourseId == e.CourseId)
                                            || (e.SectionId != null && e.Section!.StudentSections
                                               .Any(ss => ss.StudentId == studentId))))
                                .Select(e => new StudentExamDto
                                {
                                    Id = e.Id,
                                    Title = e.Name,
                                    CourseName = e.Course != null ? e.Course.Name : string.Empty,
                                    TotalMark = e.TotalMark,
                                    StartTime = e.StartTime,
                                    ExamType = e.ExamType,
                                    DurationInMinutes = e.DurationInMinutes
                                })
                                .OrderBy(e => e.StartTime)
                                .Take(3)
                                .ToList(),

                            // Sheets from enrolled courses/sections - scope to student's education year
                            Sheets = _context.Sheets
                                .Where(s => s.DueDate.HasValue &&
                                           s.DueDate > EgyptTime.UtcNow &&
                                           (
                                               (s.CourseId.HasValue &&
                                                s.Course != null &&
                                                s.Course.EducationYearId == studentEducationYearId &&
                                                _context.StudentCourses.Any(sc4 =>
                                                    sc4.StudentId == studentId &&
                                                    sc4.CourseId == s.CourseId!.Value))
                                               ||
                                               (s.SectionId.HasValue &&
                                                s.Section != null &&
                                                s.Section.Course.EducationYearId == studentEducationYearId &&
                                                (s.Section.StudentSections.Any(ss => ss.StudentId == studentId) ||
                                                 s.Section.Course.StudentCourses.Any(sc => sc.StudentId == studentId)))
                                               ||
                                               (s.VideoId.HasValue &&
                                                s.Video != null &&
                                                s.Video.Section.Course.EducationYearId == studentEducationYearId &&
                                                (s.Video.Section.StudentSections.Any(ss => ss.StudentId == studentId) ||
                                                 s.Video.Section.Course.StudentCourses.Any(sc => sc.StudentId == studentId)))
                                           ))
                                .Select(s => new StudentSheetDto
                                {
                                    Id = s.Id,
                                    Title = s.Name,
                                    CourseName = s.Course != null
                                        ? s.Course.Name
                                        : (s.Section != null
                                            ? s.Section.Course.Name
                                            : (s.Video != null ? s.Video.Section.Course.Name : string.Empty)),
                                    SheetUrl = s.SheetUrl,
                                    AnswerSheetID = s.AnswersSheets
                                                    .Where(ans => ans.StudentId == studentId)
                                                    .Select(ans => (Guid?)ans.Id)
                                                    .FirstOrDefault(),
                                    DueDate = s.DueDate
                                })
                                .OrderBy(s => s.DueDate)
                                .Take(3)
                                .ToList(),

                        };

            var result = await query.FirstOrDefaultAsync(cancellationToken);

            if (result is not null)
                result.CurrentTime = EgyptTime.UtcNow;

            return result;
        }

        public async Task<InstructorDashboardResponse?> GetInstructorDashboardDataAsync(
            Guid instructorId,
            Guid? educationYearId = null,
            CancellationToken cancellationToken = default)
        {
            // Get instructor's courses first, optionally filtered by education year
            var instructorCoursesQuery = _context.InstructorCourses
                .Where(ic => ic.InstructorId == instructorId);
            if (educationYearId.HasValue)
            {
                instructorCoursesQuery = instructorCoursesQuery.Where(ic => ic.Course.EducationYearId == educationYearId.Value);
            }
            var instructorCourseIds = await instructorCoursesQuery.Select(ic => ic.CourseId).ToListAsync(cancellationToken);

            var filteredInstructorCourses = _context.InstructorCourses
                .Where(ic => ic.InstructorId == instructorId && (!educationYearId.HasValue || ic.Course.EducationYearId == educationYearId.Value));

            var courses = await filteredInstructorCourses
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
                .ToListAsync(cancellationToken);

            var stats = new InstructorStatsDto
            {
                TotalCourses = instructorCourseIds.Count,
                TotalStudents = await filteredInstructorCourses.SumAsync(ic => ic.Course.NumberOfStudentsEnrolled, cancellationToken),
                TotalRevenue = await filteredInstructorCourses
                    .SelectMany(ic => ic.Course.Payments)
                    .Where(p => p.Status == PaymentStatus.Completed)
                    .SumAsync(p => p.Amount, cancellationToken),
                AverageRating = await filteredInstructorCourses.AverageAsync(ic => ic.Course.Rating ?? 0, cancellationToken),
                TotalVideos = await filteredInstructorCourses.SumAsync(ic => ic.Course.NumberOfVideos, cancellationToken),
                TotalExams = await filteredInstructorCourses.SumAsync(ic => ic.Course.NumberOfExams, cancellationToken),
                TotalSheets = await filteredInstructorCourses.SumAsync(ic => ic.Course.NumberOfQuestionSheets, cancellationToken)
            };

            InstructorDashboardResponse? response = new()
            {
                Courses = courses,
                Stats = [stats]
            };

            // Recent activities
            var videos = _context.Videos
                .Where(v => instructorCourseIds.Contains(v.Section!.CourseId) && (!educationYearId.HasValue || v.Section.Course.EducationYearId == educationYearId.Value))
                .Select(v => new RecentActivityDto
                {
                    ActivityType = "Video",
                    Description = v.Name,
                    CourseName = v.Section!.Course!.Name,
                    Timestamp = v.CreatedAt,
                    EntityUrl = v.VideoUrl,
                    RelatedEntityId = v.Id
                });

            var exams = _context.Exams
                .Where(e => e.InstructorId == instructorId && (!educationYearId.HasValue || e.Course.EducationYearId == educationYearId.Value))
                .Select(e => new RecentActivityDto
                {
                    ActivityType = "Exam",
                    Description = e.Name,
                    CourseName = e.Course!.Name,
                    Timestamp = e.CreatedAt,
                    RelatedEntityId = e.Id
                });

            var sheets = _context.Sheets
                .Where(s => s.InstructorId == instructorId && (!educationYearId.HasValue || (s.Course != null && s.Course.EducationYearId == educationYearId.Value)))
                .Select(s => new RecentActivityDto
                {
                    ActivityType = "Sheet",
                    Description = s.Name,
                    CourseName = s.Course!.Name,
                    Timestamp = s.CreatedAt,
                    EntityUrl = s.SheetUrl,
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

            // Materialize each query before performing set operations in-memory to avoid EF Core translation issues
            var videosList = await videos.ToListAsync(cancellationToken);
            var examsList = await exams.ToListAsync(cancellationToken);
            var sheetsList = await sheets.ToListAsync(cancellationToken);
            var enrollmentsList = await enrollments.ToListAsync(cancellationToken);

            response.RecentActivities = [.. videosList.Concat(examsList).Concat(sheetsList).Concat(enrollmentsList)
                //.OrderByDescending()
                .Take(10)];

            var instructorNow = EgyptTime.UtcNow;

            var sheetsToReview = _context.Sheets
                .Where(s => s.InstructorId == instructorId && (!educationYearId.HasValue || (s.Course != null && s.Course.EducationYearId == educationYearId.Value)) && s.DueDate.HasValue &&
                           s.DueDate.Value <= instructorNow.AddDays(3))
                .Select(s => new PendingTaskDto
                {
                    TaskType = "Review",
                    Title = $"Review sheet: {s.Name}",
                    CourseName = s.Course!.Name,
                    DueDate = s.DueDate!.Value,
                    Priority = 2,
                    RelatedEntityId = s.Id
                });

            // Materialize each query before performing set operations in-memory to avoid EF Core translation issues
            var sheetsToReviewList = await sheetsToReview.ToListAsync(cancellationToken);

            response.PendingTasks = [.. sheetsToReviewList
                                    .OrderBy(t => t.Priority)
                                    .ThenBy(t => t.DueDate)
                                    .Take(8)];

            response.UpcomingExams = await _context.Exams
                .Where(e => e.InstructorId == instructorId && (!educationYearId.HasValue || e.Course.EducationYearId == educationYearId.Value) && e.StartTime.HasValue && e.StartTime.Value >= instructorNow)
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
                .ToListAsync(cancellationToken);

            response.UpcomingSheets = await _context.Sheets
                .Where(s => s.InstructorId == instructorId && (!educationYearId.HasValue || (s.Course != null && s.Course.EducationYearId == educationYearId.Value)) && s.DueDate.HasValue && s.DueDate.Value >= instructorNow.AddDays(-7))
                .OrderBy(s => s.DueDate)
                .Select(s => new UpcomingSheetDto
                {
                    Id = s.Id,
                    Title = s.Name,
                    SheetUrl = s.SheetUrl,
                    CourseName = s.Course!.Name,
                    DueDate = s.DueDate,
                    Status = "Published",
                    NumberOfSubmittedStudents = s.AnswersSheets.Count
                })
                .Take(5)
                .ToListAsync(cancellationToken);

            response.CurrentTime = EgyptTime.UtcNow;

            return response;
        }
    }
}
