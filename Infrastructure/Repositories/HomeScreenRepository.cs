using Application.DTOs.HomeScreen;
using Application.Interfaces;
using Application.ResultWrapper;
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
            var studentEducationYearId = await _context.Students
                .AsNoTracking()
                .Where(s => s.UserId == studentId)
                .Select(s => s.EducationYearId)
                .FirstOrDefaultAsync(cancellationToken);

            var query = from sc in _context.StudentCourses
                        where sc.StudentId == studentId
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
                                .Where(v => v.Section != null && v.Section.Course.EducationYearId == studentEducationYearId)
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
                                           e.Course.EducationYearId == studentEducationYearId &&
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

                            // Sheets from enrolled courses - scope to student's education year
                            Sheets = _context.Sheets
                                .Where(s => s.CourseId.HasValue &&
                                           s.Course != null &&
                                           s.Course.EducationYearId == studentEducationYearId &&
                                           s.DueDate.HasValue &&
                                           _context.StudentCourses
                                               .Any(sc4 => sc4.StudentId == studentId &&
                                                        sc4.CourseId == s.CourseId!.Value))
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

        public async Task<StudentProgressSummaryResponse> GetStudentProgressDataAsync(
            Guid studentId,
            int coursesPage,
            int coursesPageSize,
            int milestonesPage,
            int milestonesPageSize,
            CancellationToken cancellationToken = default)
        {
            coursesPage = coursesPage < 1 ? 1 : coursesPage;
            coursesPageSize = coursesPageSize <= 0 ? 6 : coursesPageSize;
            milestonesPage = milestonesPage < 1 ? 1 : milestonesPage;
            milestonesPageSize = milestonesPageSize <= 0 ? 10 : milestonesPageSize;

            var studentCoursesQuery = _context.StudentCourses
                .Include(sc => sc.Course)
                .Where(sc => sc.StudentId == studentId);

            var studentCoursesAll = await studentCoursesQuery.ToListAsync(cancellationToken);

            var inProgressCoursesCount = studentCoursesAll
                .Count(sc =>
                    sc.Course.NumberOfVideos > 0 &&
                    sc.NumberOfCourseVideosWatched < sc.Course.NumberOfVideos);

            var completedLessonsCount = studentCoursesAll.Sum(sc => sc.NumberOfCourseVideosWatched);

            var courseDtosAll = studentCoursesAll
                .Select(sc => new StudentProgressCourseDto
                {
                    Id = sc.CourseId,
                    Name = sc.Course.Name,
                    PictureUrl = sc.Course.PictureUrl,
                    TotalLessons = sc.Course.NumberOfVideos,
                    CompletedLessons = sc.NumberOfCourseVideosWatched,
                    GradePercent = null
                })
                .OrderBy(c => c.Name)
                .ToList();

            var coursesTotalCount = courseDtosAll.Count;
            var coursesItems = courseDtosAll
                .Skip((coursesPage - 1) * coursesPageSize)
                .Take(coursesPageSize)
                .ToList();

            var paginatedCourses = new PaginatedResult<StudentProgressCourseDto>
            {
                Items = coursesItems,
                PageNumber = coursesPage,
                PageSize = coursesPageSize,
                TotalCount = coursesTotalCount
            };

            // Average grade across completed exams (global IsDeleted filter applied)
            var examResults = await _context.ExamResults
                .Include(er => er.Exam)
                .Where(er =>
                    er.StudentId == studentId &&
                    er.StudentMark.HasValue &&
                    er.Exam.TotalMark > 0)
                .ToListAsync(cancellationToken);

            var averageGrade = examResults.Count > 0
                ? examResults.Average(er => (er.StudentMark!.Value / er.Exam.TotalMark) * 100m)
                : 0m;

            var upcomingExamsQuery = _context.Exams
                .Where(e =>
                    e.StartTime.HasValue &&
                    e.StartTime.Value >= DateTimeOffset.UtcNow &&
                    _context.StudentCourses.Any(sc =>
                        sc.StudentId == studentId &&
                        sc.CourseId == e.CourseId))
                .Select(e => new UpcomingMilestoneDto
                {
                    Id = e.Id,
                    Title = e.Name,
                    CourseName = e.Course != null ? e.Course.Name : string.Empty,
                    Type = "Exam",
                    DueAt = e.StartTime!.Value
                });

            var upcomingSheetsQuery = _context.Sheets
                .Where(s =>
                    s.CourseId.HasValue &&
                    s.DueDate.HasValue &&
                    s.DueDate.Value >= DateTimeOffset.UtcNow &&
                    _context.StudentCourses.Any(sc =>
                        sc.StudentId == studentId &&
                        sc.CourseId == s.CourseId.Value))
                .Select(s => new UpcomingMilestoneDto
                {
                    Id = s.Id,
                    Title = s.Name,
                    CourseName = s.Course != null ? s.Course.Name : string.Empty,
                    Type = "Sheet",
                    DueAt = s.DueDate!.Value
                });

            var milestonesQuery = upcomingExamsQuery.Concat(upcomingSheetsQuery);
            var milestonesTotalCount = await milestonesQuery.CountAsync(cancellationToken);
            var milestonesItems = await milestonesQuery
                .OrderBy(m => m.DueAt)
                .Skip((milestonesPage - 1) * milestonesPageSize)
                .Take(milestonesPageSize)
                .ToListAsync(cancellationToken);

            var paginatedMilestones = new PaginatedResult<UpcomingMilestoneDto>
            {
                Items = milestonesItems,
                PageNumber = milestonesPage,
                PageSize = milestonesPageSize,
                TotalCount = milestonesTotalCount
            };

            return new StudentProgressSummaryResponse
            {
                InProgressCoursesCount = inProgressCoursesCount,
                CompletedLessonsCount = completedLessonsCount,
                AverageGrade = averageGrade,
                Courses = paginatedCourses,
                UpcomingMilestones = paginatedMilestones
            };
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

            InstructorDashboardResponse? response = new()
            {
                // Instructor's courses with metrics
                Courses = _context.InstructorCourses
                    .Where(ic => ic.InstructorId == instructorId && (!educationYearId.HasValue || ic.Course.EducationYearId == educationYearId.Value))
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
                            .Where(ic => ic.InstructorId == instructorId && (!educationYearId.HasValue || ic.Course.EducationYearId == educationYearId.Value))
                            .Sum(ic => ic.Course.NumberOfStudentsEnrolled),
                        TotalRevenue = _context.InstructorCourses
                            .Where(ic => ic.InstructorId == instructorId && (!educationYearId.HasValue || ic.Course.EducationYearId == educationYearId.Value))
                            .SelectMany(ic => ic.Course.Payments)
                            .Where(p => p.Status == PaymentStatus.Completed)
                            .Sum(p => p.Amount),
                        AverageRating = _context.InstructorCourses
                            .Where(ic => ic.InstructorId == instructorId && (!educationYearId.HasValue || ic.Course.EducationYearId == educationYearId.Value))
                            .Average(ic => ic.Course.Rating ?? 0),
                        TotalVideos = _context.InstructorCourses
                            .Where(ic => ic.InstructorId == instructorId && (!educationYearId.HasValue || ic.Course.EducationYearId == educationYearId.Value))
                            .Sum(ic => ic.Course.NumberOfVideos),
                        TotalExams = _context.InstructorCourses
                            .Where(ic => ic.InstructorId == instructorId && (!educationYearId.HasValue || ic.Course.EducationYearId == educationYearId.Value))
                            .Sum(ic => ic.Course.NumberOfExams),
                        TotalSheets = _context.InstructorCourses
                            .Where(ic => ic.InstructorId == instructorId && (!educationYearId.HasValue || ic.Course.EducationYearId == educationYearId.Value))
                            .Sum(ic => ic.Course.NumberOfQuestionSheets)
                    }
                }
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

            response.RecentActivities = videosList.Concat(examsList).Concat(sheetsList).Concat(enrollmentsList)
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .ToList();

            var sheetsToReview = _context.Sheets
                .Where(s => s.InstructorId == instructorId && (!educationYearId.HasValue || (s.Course != null && s.Course.EducationYearId == educationYearId.Value)) && s.DueDate.HasValue &&
                           s.DueDate.Value <= DateTimeOffset.UtcNow.AddDays(3))
                .Select(s => new PendingTaskDto
                {
                    TaskType = "Review",
                    Title = $"Review sheet: {s.Name}",
                    CourseName = s.Course!.Name,
                    DueDate = s.DueDate!.Value,
                    Priority = 2,
                    RelatedEntityId = s.Id
                });

            //var supportTasks = _context.StudentCourses
            //    .Where(sc => instructorCourseIds.Contains(sc.CourseId) && sc.EnrolledAt >= DateTimeOffset.UtcNow.AddDays(-7))
            //    .Select(sc => new PendingTaskDto
            //    {
            //        TaskType = "Support",
            //        Title = "Answer student questions",
            //        CourseName = sc.Course.Name,
            //        DueDate = null,
            //        Priority = 3,
            //        RelatedEntityId = null
            //    });

            // Materialize each query before performing set operations in-memory to avoid EF Core translation issues
            var sheetsToReviewList = await sheetsToReview.ToListAsync(cancellationToken);
            //var supportTasksList = await supportTasks.ToListAsync(cancellationToken);

            //response.PendingTasks = sheetsToReviewList.Concat(supportTasksList)
            //    .OrderBy(t => t.Priority)
            //    .ThenBy(t => t.DueDate ?? DateTimeOffset.MaxValue)
            //    .Take(8)
            //    .ToList();

            response.PendingTasks = sheetsToReviewList
                                    .OrderBy(t => t.Priority)
                                    .ThenBy(t => t.DueDate)
                                    .Take(8)
                                    .ToList();

            // Upcoming exams
            response.UpcomingExams = _context.Exams
                .Where(e => e.InstructorId == instructorId && (!educationYearId.HasValue || e.Course.EducationYearId == educationYearId.Value) && e.StartTime.HasValue && e.StartTime.Value >= DateTimeOffset.UtcNow)
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
                .Where(s => s.InstructorId == instructorId && (!educationYearId.HasValue || (s.Course != null && s.Course.EducationYearId == educationYearId.Value)) && s.DueDate.HasValue && s.DueDate.Value >= DateTimeOffset.UtcNow.AddDays(-7))
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
