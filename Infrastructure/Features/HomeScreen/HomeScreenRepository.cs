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
                                .Where(e => e.Status != ExamStatus.Draft &&
                                           e.StartTime.HasValue &&
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

        public async Task<InstructorScheduleResponse> GetInstructorScheduleAsync(
            Guid instructorId, Guid educationYearId, int days,
            CancellationToken cancellationToken = default)
        {
            var now = EgyptTime.UtcNow;
            var cutoff = now.AddDays(days);

            var examItems = await (
                from e in _context.Exams
                join c in _context.Courses on e.CourseId equals c.Id
                where e.InstructorId == instructorId
                      && c.EducationYearId == educationYearId
                      && e.StartTime.HasValue
                      && e.StartTime.Value >= now
                      && e.StartTime.Value <= cutoff
                      && e.Status != ExamStatus.Draft
                select new ScheduleItemDto
                {
                    Id = e.Id,
                    Type = "Exam",
                    Title = e.Name,
                    StartOrDue = e.StartTime,
                    DurationInMinutes = e.DurationInMinutes,
                    CourseName = c.Name,
                    EnrolledCount = e.SectionId == null ? c.NumberOfStudentsEnrolled
                        : _context.Sections.Where(s => s.Id == e.SectionId).Select(s => s.NumberOfStudentsEnrolled).FirstOrDefault(),
                    SubmittedCount = 0
                }).ToListAsync(cancellationToken);

            var sheetItems = await (
                from s in _context.Sheets
                where s.InstructorId == instructorId
                      && s.DueDate.HasValue
                      && s.DueDate.Value >= now
                      && s.DueDate.Value <= cutoff
                      && ((s.CourseId != null && _context.Courses.Any(c => c.Id == s.CourseId && c.EducationYearId == educationYearId))
                          || (s.SectionId != null && _context.Sections.Any(sec => sec.Id == s.SectionId && sec.Course.EducationYearId == educationYearId)))
                select new ScheduleItemDto
                {
                    Id = s.Id,
                    Type = "Sheet",
                    Title = s.Name,
                    StartOrDue = s.DueDate,
                    DurationInMinutes = null,
                    CourseName = s.CourseId != null
                        ? _context.Courses.Where(c => c.Id == s.CourseId).Select(c => c.Name).FirstOrDefault() ?? ""
                        : s.SectionId != null
                        ? _context.Sections.Where(sec => sec.Id == s.SectionId).Select(sec => sec.Course.Name).FirstOrDefault() ?? "" : "",
                    EnrolledCount = s.CourseId != null
                        ? _context.Courses.Where(c => c.Id == s.CourseId).Select(c => c.NumberOfStudentsEnrolled).FirstOrDefault()
                        : s.SectionId != null
                        ? _context.Sections.Where(sec => sec.Id == s.SectionId).Select(sec => sec.NumberOfStudentsEnrolled).FirstOrDefault()
                        : 0,
                    SubmittedCount = s.AnswersSheets.Count()
                }).ToListAsync(cancellationToken);

            var days_grouped = examItems.Concat(sheetItems)
                .GroupBy(i => i.StartOrDue!.Value.Date.ToString("yyyy-MM-dd"))
                .OrderBy(g => g.Key)
                .Select(g => new ScheduleDayDto
                {
                    Date = g.Key,
                    Items = [.. g.OrderBy(i => i.StartOrDue)]
                })
                .ToList();

            return new InstructorScheduleResponse { Days = days_grouped };
        }

        public async Task<AttentionResponse> GetInstructorAttentionAsync(
            Guid instructorId, Guid educationYearId,
            CancellationToken cancellationToken = default)
        {
            const int cap = 20;
            var now = EgyptTime.UtcNow;
            var monthStart = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);

            var courseIds = await _context.InstructorCourses
                .Where(ic => ic.InstructorId == instructorId && ic.Course.EducationYearId == educationYearId)
                .Select(ic => ic.CourseId)
                .ToListAsync(cancellationToken);

            var items = new List<AttentionItemDto>();

            // 1. Sheet submitted, not approved
            var pendingSheets = await _context.AnswersSheets
                .AsNoTracking()
                .Where(a => !a.IsApproved
                    && a.QuestionsSheet.InstructorId == instructorId
                    && (a.QuestionsSheet.Course != null && a.QuestionsSheet.Course.EducationYearId == educationYearId
                        || a.QuestionsSheet.Section != null && a.QuestionsSheet.Section.Course.EducationYearId == educationYearId))
                .OrderByDescending(a => a.CreatedAt)
                .Take(cap)
                .Select(a => new AttentionItemDto
                {
                    Type = "SheetPendingApproval",
                    StudentId = a.StudentId,
                    StudentName = a.Student.User.FullName,
                    CourseName = a.QuestionsSheet.Course != null ? a.QuestionsSheet.Course.Name
                        : a.QuestionsSheet.Section != null ? a.QuestionsSheet.Section.Course.Name : string.Empty,
                    EntityId = a.QuestionsSheetId,
                    Title = a.QuestionsSheet.Name,
                    When = a.CreatedAt,
                    ParentPhone = a.Student.ParentPhoneNumber
                })
                .ToListAsync(cancellationToken);
            items.AddRange(pendingSheets);

            // 2. Sheet overdue, student did not submit (enrolled in instructor's courses)
            var overdueUnsubmitted = await (
                from s in _context.Sheets
                where s.InstructorId == instructorId
                      && s.DueDate.HasValue && s.DueDate.Value < now
                      && (s.Course != null && s.Course.EducationYearId == educationYearId
                          || s.Section != null && s.Section.Course.EducationYearId == educationYearId)
                from sc in _context.StudentCourses
                where s.CourseId.HasValue && sc.CourseId == s.CourseId.Value
                      && !s.AnswersSheets.Any(a => a.StudentId == sc.StudentId)
                select new AttentionItemDto
                {
                    Type = "SheetOverdue",
                    StudentId = sc.StudentId,
                    StudentName = sc.Student.User.FullName,
                    CourseName = s.Course != null ? s.Course.Name : string.Empty,
                    EntityId = s.Id,
                    Title = s.Name,
                    When = s.DueDate!.Value,
                    ParentPhone = sc.Student.ParentPhoneNumber
                })
                .OrderByDescending(i => i.When)
                .Take(cap)
                .ToListAsync(cancellationToken);
            items.AddRange(overdueUnsubmitted);

            // 3. Failed exams
            var failedExams = await _context.ExamResults
                .AsNoTracking()
                .Where(er => er.Status == ExamResultStatus.Failed
                    && er.Exam.InstructorId == instructorId
                    && er.Exam.Course.EducationYearId == educationYearId)
                .OrderByDescending(er => er.CreatedAt)
                .Take(cap)
                .Select(er => new AttentionItemDto
                {
                    Type = "ExamFailed",
                    StudentId = er.StudentId,
                    StudentName = er.Student.User.FullName,
                    CourseName = er.Exam.Course.Name,
                    EntityId = er.ExamId,
                    Title = er.Exam.Name,
                    When = er.CreatedAt,
                    ParentPhone = er.Student.ParentPhoneNumber
                })
                .ToListAsync(cancellationToken);
            items.AddRange(failedExams);

            // 4. TriedScreenshot = true
            var screenshots = await _context.Students
                .AsNoTracking()
                .Where(s => s.TriedScreenshot
                    && s.StudentCourses.Any(sc => courseIds.Contains(sc.CourseId)))
                .Take(cap)
                .Select(s => new AttentionItemDto
                {
                    Type = "ScreenshotFlag",
                    StudentId = s.UserId,
                    StudentName = s.User.FullName,
                    CourseName = s.StudentCourses
                        .Where(sc => courseIds.Contains(sc.CourseId))
                        .Select(sc => sc.Course.Name)
                        .FirstOrDefault() ?? string.Empty,
                    EntityId = s.UserId,
                    Title = "Screenshot attempt detected",
                    When = EgyptTime.UtcNow,
                    ParentPhone = s.ParentPhoneNumber
                })
                .ToListAsync(cancellationToken);
            items.AddRange(screenshots);

            // 5. Enrollments this month
            var newEnrollments = await _context.StudentCourses
                .AsNoTracking()
                .Where(sc => courseIds.Contains(sc.CourseId) && sc.EnrolledAt >= monthStart)
                .OrderByDescending(sc => sc.EnrolledAt)
                .Take(cap)
                .Select(sc => new AttentionItemDto
                {
                    Type = "NewEnrollment",
                    StudentId = sc.StudentId,
                    StudentName = sc.Student.User.FullName,
                    CourseName = sc.Course.Name,
                    EntityId = sc.CourseId,
                    Title = "New enrollment",
                    When = sc.EnrolledAt,
                    ParentPhone = sc.Student.ParentPhoneNumber
                })
                .ToListAsync(cancellationToken);
            items.AddRange(newEnrollments);

            return new AttentionResponse
            {
                Items = [.. items.OrderByDescending(i => i.When).Take(cap)]
            };
        }

        public async Task<AtRiskResponse> GetInstructorAtRiskAsync(
            Guid instructorId, Guid educationYearId, int page, int pageSize,
            CancellationToken cancellationToken = default)
        {
            var now = EgyptTime.UtcNow;
            var sevenDaysAgo = now.AddDays(-7);

            var courseIds = await _context.InstructorCourses
                .Where(ic => ic.InstructorId == instructorId && ic.Course.EducationYearId == educationYearId)
                .Select(ic => ic.CourseId)
                .ToListAsync(cancellationToken);

            var sectionIds = await _context.InstructorSections
                .Where(isec => isec.InstructorId == instructorId && courseIds.Contains(isec.Section.CourseId))
                .Select(isec => isec.SectionId)
                .ToListAsync(cancellationToken);

            var examIds = await _context.Exams
                .Where(e => e.InstructorId == instructorId && e.Course.EducationYearId == educationYearId)
                .Select(e => e.Id)
                .ToListAsync(cancellationToken);

            var overdueSheetIds = await _context.Sheets
                .Where(s => s.InstructorId == instructorId
                    && s.DueDate.HasValue && s.DueDate.Value < now
                    && (s.Course != null && s.Course.EducationYearId == educationYearId
                        || s.Section != null && s.Section.Course.EducationYearId == educationYearId))
                .Select(s => s.Id)
                .ToListAsync(cancellationToken);

            // Enrolled student IDs
            var courseStudentIds = await _context.StudentCourses
                .Where(sc => courseIds.Contains(sc.CourseId))
                .Select(sc => sc.StudentId)
                .ToListAsync(cancellationToken);

            var sectionStudentIds = await _context.StudentSections
                .Where(ss => sectionIds.Contains(ss.SectionId))
                .Select(ss => ss.StudentId)
                .ToListAsync(cancellationToken);

            var enrolledStudentIds = courseStudentIds.Union(sectionStudentIds).Distinct().ToList();

            if (enrolledStudentIds.Count == 0)
                return new AtRiskResponse { Students = [], TotalCount = 0, Page = page, PageSize = pageSize };

            var lastWatchedMap = await _context.StudentVideos
                .Where(sv => enrolledStudentIds.Contains(sv.StudentId)
                    && sv.Video.Section != null
                    && (courseIds.Contains(sv.Video.Section.CourseId) || sectionIds.Contains(sv.Video.SectionId)))
                .GroupBy(sv => sv.StudentId)
                .Select(g => new { StudentId = g.Key, LastSeen = g.Max(sv => sv.WatchedAt) })
                .ToDictionaryAsync(x => x.StudentId, x => x.LastSeen, cancellationToken);

            var failedMap = await _context.ExamResults
                .Where(er => enrolledStudentIds.Contains(er.StudentId)
                    && examIds.Contains(er.ExamId)
                    && er.Status == ExamResultStatus.Failed)
                .GroupBy(er => er.StudentId)
                .Select(g => new { StudentId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.StudentId, x => x.Count, cancellationToken);

            // Per-student overdue unsubmitted sheet count
            var overdueMap = new Dictionary<Guid, int>();
            if (overdueSheetIds.Count > 0)
            {
                var overdueEntries = await (
                    from s in _context.Sheets
                    where overdueSheetIds.Contains(s.Id)
                    from sc in _context.StudentCourses
                    where s.CourseId.HasValue && sc.CourseId == s.CourseId.Value
                          && enrolledStudentIds.Contains(sc.StudentId)
                          && !s.AnswersSheets.Any(a => a.StudentId == sc.StudentId)
                    select sc.StudentId)
                    .ToListAsync(cancellationToken);

                foreach (var sid in overdueEntries)
                    overdueMap[sid] = overdueMap.GetValueOrDefault(sid, 0) + 1;
            }

            var atRiskIds = enrolledStudentIds.Where(sid =>
            {
                var inactive = !lastWatchedMap.TryGetValue(sid, out var lastSeen) || lastSeen < sevenDaysAgo;
                var failedExams = failedMap.TryGetValue(sid, out var fc) && fc >= 2;
                var overdueSheets = overdueMap.TryGetValue(sid, out var oc) && oc >= 1;
                return inactive || failedExams || overdueSheets;
            }).ToList();

            var totalCount = atRiskIds.Count;
            var safePage = Math.Max(1, page);
            var safePageSize = Math.Clamp(pageSize, 1, 50);
            var pagedIds = atRiskIds.Skip((safePage - 1) * safePageSize).Take(safePageSize).ToList();

            var studentDetails = await _context.Students
                .AsNoTracking()
                .Where(s => pagedIds.Contains(s.UserId))
                .Select(s => new { s.UserId, s.User.FullName, s.User.PersonalPictureUrl, s.ParentPhoneNumber })
                .ToListAsync(cancellationToken);

            var students = studentDetails.Select(s =>
            {
                var inactive = !lastWatchedMap.TryGetValue(s.UserId, out var lastSeen) || lastSeen < sevenDaysAgo;
                var fc = failedMap.GetValueOrDefault(s.UserId, 0);
                var oc = overdueMap.GetValueOrDefault(s.UserId, 0);
                var reason = inactive ? "Inactive" : fc >= 2 ? "FailedExams" : "OverdueSheet";

                return new AtRiskStudentDto
                {
                    StudentId = s.UserId,
                    StudentName = s.FullName,
                    PictureUrl = s.PersonalPictureUrl,
                    ParentPhone = s.ParentPhoneNumber,
                    Reason = reason,
                    LastSeenAt = lastWatchedMap.GetValueOrDefault(s.UserId),
                    FailedExamCount = fc,
                    OverdueSheetCount = oc
                };
            }).ToList();

            return new AtRiskResponse
            {
                Students = students,
                TotalCount = totalCount,
                Page = safePage,
                PageSize = safePageSize
            };
        }

        public async Task<InstructorPaymentsResponse> GetInstructorPaymentsAsync(
            Guid instructorId, Guid educationYearId, int days,
            CancellationToken cancellationToken = default)
        {
            var now = EgyptTime.UtcNow;
            var since = now.AddDays(-days);

            var courseIds = await _context.InstructorCourses
                .Where(ic => ic.InstructorId == instructorId && ic.Course.EducationYearId == educationYearId)
                .Select(ic => ic.CourseId)
                .ToListAsync(cancellationToken);

            var sectionIds = await _context.InstructorSections
                .Where(isec => isec.InstructorId == instructorId && courseIds.Contains(isec.Section.CourseId))
                .Select(isec => isec.SectionId)
                .ToListAsync(cancellationToken);

            var paymentsQuery = _context.Payments
                .AsNoTracking()
                .Where(p => p.CreatedAt >= since
                    && ((p.CourseId.HasValue && courseIds.Contains(p.CourseId.Value))
                        || (p.SectionId.HasValue && sectionIds.Contains(p.SectionId.Value))));

            var completedAmount = await paymentsQuery
                .Where(p => p.Status == PaymentStatus.Completed)
                .SumAsync(p => p.Amount, cancellationToken);

            var pendingCount = await paymentsQuery
                .CountAsync(p => p.Status == PaymentStatus.Pending, cancellationToken);

            var failedCount = await paymentsQuery
                .CountAsync(p => p.Status == PaymentStatus.Failed, cancellationToken);

            var rows = await paymentsQuery
                .OrderByDescending(p => p.CreatedAt)
                .Take(20)
                .Select(p => new PaymentRowDto
                {
                    StudentId = p.StudentId,
                    StudentName = p.Student != null ? p.Student.User.FullName : string.Empty,
                    EntityName = p.CourseId.HasValue && p.Course != null ? p.Course.Name
                        : p.SectionId.HasValue && p.Section != null ? p.Section.Name : string.Empty,
                    Amount = p.Amount,
                    Status = p.Status.ToString(),
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return new InstructorPaymentsResponse
            {
                CompletedAmount = completedAmount,
                PendingCount = pendingCount,
                FailedCount = failedCount,
                Days = days,
                Rows = rows
            };
        }
    }
}

