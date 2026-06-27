using Application.DTOs.HomeScreen;
using Domain.enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Progress
{
    public sealed class EnrollmentProgressHelper(EducationDbContext context)
    {
        public const int MaxInstructorPageSize = 25;

        private readonly EducationDbContext _context = context;

        public async Task<int> CountEnrollmentStubsForStudentAsync(
            Guid studentId,
            EnrollmentProgressScope scope,
            CancellationToken cancellationToken = default)
        {
            var courseQuery = _context.StudentCourses
                .AsNoTracking()
                .Where(sc => sc.StudentId == studentId);

            if (scope.AllowedCourseIds is not null)
                courseQuery = courseQuery.Where(sc => scope.AllowedCourseIds.Contains(sc.CourseId));

            var sectionQuery = _context.StudentSections
                .AsNoTracking()
                .Where(ss => ss.StudentId == studentId)
                .Where(ss => !_context.StudentCourses.Any(sc =>
                    sc.StudentId == studentId && sc.CourseId == ss.Section.CourseId));

            if (scope.AllowedSectionIds is not null)
                sectionQuery = sectionQuery.Where(ss => scope.AllowedSectionIds.Contains(ss.SectionId));

            if (scope.AllowedCourseIds is not null)
                sectionQuery = sectionQuery.Where(ss => scope.AllowedCourseIds.Contains(ss.Section.CourseId));

            return await courseQuery.CountAsync(cancellationToken)
                   + await sectionQuery.CountAsync(cancellationToken);
        }

        internal async Task<List<EnrollmentListItem>> GetEnrollmentStubsForStudentAsync(
            Guid studentId,
            EnrollmentProgressScope scope,
            CancellationToken cancellationToken = default)
        {
            var courseEnrollments = await QueryCourseStubs([studentId], scope, cancellationToken);
            var sectionOnlyEnrollments = await QuerySectionOnlyStubs([studentId], scope, cancellationToken);

            return courseEnrollments
                .Concat(sectionOnlyEnrollments)
                .OrderByDescending(e => e.EnrolledAt)
                .ToList();
        }

        internal async Task<List<EnrollmentListItem>> GetEnrollmentStubPageForStudentAsync(
            Guid studentId,
            EnrollmentProgressScope scope,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var all = await GetEnrollmentStubsForStudentAsync(studentId, scope, cancellationToken);
            return all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<Dictionary<Guid, List<EnrollmentProgressDto>>> BuildEnrollmentProgressForStudentsAsync(
            IReadOnlyList<Guid> studentIds,
            EnrollmentProgressScope scope,
            CancellationToken cancellationToken = default)
        {
            if (studentIds.Count == 0)
                return new Dictionary<Guid, List<EnrollmentProgressDto>>();

            var courseStubs = await QueryCourseStubs(studentIds, scope, cancellationToken);
            var sectionOnlyStubs = await QuerySectionOnlyStubs(studentIds, scope, cancellationToken);
            var allStubs = courseStubs.Concat(sectionOnlyStubs).ToList();

            var stubsByStudent = allStubs
                .GroupBy(s => s.StudentId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(e => e.EnrolledAt).ToList());

            var courseIds = allStubs.Select(s => s.CourseId).Distinct().ToList();
            var sectionIds = allStubs
                .Where(s => s.SectionId.HasValue)
                .Select(s => s.SectionId!.Value)
                .Distinct()
                .ToList();

            var subscribedSections = await LoadSubscribedSectionsAsync(studentIds, courseIds, scope, cancellationToken);
            sectionIds = sectionIds
                .Concat(subscribedSections.Select(s => s.SectionId))
                .Distinct()
                .ToList();

            var examRows = await LoadExamProgressRowsAsync(studentIds, courseIds, cancellationToken);
            var sheetRows = await LoadSheetProgressRowsAsync(studentIds, courseIds, sectionIds, cancellationToken);

            var result = new Dictionary<Guid, List<EnrollmentProgressDto>>();
            foreach (var studentId in studentIds)
            {
                if (!stubsByStudent.TryGetValue(studentId, out var stubs))
                {
                    result[studentId] = [];
                    continue;
                }

                var studentExams = examRows.Where(e => e.StudentId == studentId).ToList();
                var studentSheets = sheetRows.Where(s => s.StudentId == studentId).ToList();
                var studentSections = subscribedSections.Where(s => s.StudentId == studentId).ToList();

                result[studentId] = stubs
                    .Select(e => EnrollmentProgressMapper.MapEnrollmentProgress(e, studentExams, studentSheets, studentSections))
                    .ToList();
            }

            return result;
        }

        internal async Task<List<EnrollmentProgressDto>> BuildEnrollmentProgressDtosAsync(
            Guid studentId,
            IReadOnlyList<EnrollmentListItem> enrollments,
            EnrollmentProgressScope scope,
            CancellationToken cancellationToken = default)
        {
            if (enrollments.Count == 0)
                return [];

            var map = await BuildEnrollmentProgressForStudentsAsync([studentId], scope, cancellationToken);
            if (!map.TryGetValue(studentId, out var dtos))
                return [];

            var keys = enrollments
                .Select(e => (Type: e.Type.ToApiValue(), e.CourseId, e.SectionId))
                .ToHashSet();

            return dtos
                .Where(d => keys.Contains((d.EnrollmentType, d.CourseId, d.SectionId)))
                .ToList();
        }

        /// <summary>
        /// Lightweight rows for instructor student pagination (no progress aggregates).
        /// </summary>
        public async Task<(int TotalCount, List<InstructorStudentSummary> Page)> GetInstructorStudentPageAsync(
            HashSet<Guid> courseIds,
            HashSet<Guid> sectionIds,
            Guid? studentId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var courseLight = courseIds.Count > 0
                ? await _context.StudentCourses
                    .AsNoTracking()
                    .Where(sc => courseIds.Contains(sc.CourseId))
                    .Where(sc => !studentId.HasValue || sc.StudentId == studentId)
                    .Select(sc => new { sc.StudentId, sc.EnrolledAt })
                    .ToListAsync(cancellationToken)
                : [];

            var sectionLight = sectionIds.Count > 0
                ? await _context.StudentSections
                    .AsNoTracking()
                    .Where(ss => sectionIds.Contains(ss.SectionId))
                    .Where(ss => !studentId.HasValue || ss.StudentId == studentId)
                    .Where(ss => !_context.StudentCourses.Any(sc =>
                        sc.StudentId == ss.StudentId &&
                        courseIds.Contains(sc.CourseId) &&
                        sc.CourseId == ss.Section.CourseId))
                    .Select(ss => new { ss.StudentId, ss.EnrolledAt })
                    .ToListAsync(cancellationToken)
                : [];

            var grouped = courseLight
                .Concat(sectionLight)
                .GroupBy(x => x.StudentId)
                .Select(g => new { StudentId = g.Key, LastEnrolledAt = g.Max(x => x.EnrolledAt) })
                .OrderByDescending(x => x.LastEnrolledAt)
                .ToList();

            var totalCount = grouped.Count;
            var pageIds = grouped
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.StudentId)
                .ToList();

            if (pageIds.Count == 0)
                return (totalCount, []);

            var summaries = await _context.Students
                .AsNoTracking()
                .Where(s => pageIds.Contains(s.UserId))
                .Select(s => new InstructorStudentSummary
                {
                    StudentId = s.UserId,
                    StudentName = s.User.FullName,
                    StudentEmail = s.User.GmailExternal
                })
                .ToListAsync(cancellationToken);

            var orderMap = pageIds.Select((id, index) => (id, index)).ToDictionary(x => x.id, x => x.index);
            summaries.Sort((a, b) => orderMap[a.StudentId].CompareTo(orderMap[b.StudentId]));

            return (totalCount, summaries);
        }

        private async Task<List<EnrollmentListItem>> QueryCourseStubs(
            IReadOnlyList<Guid> studentIds,
            EnrollmentProgressScope scope,
            CancellationToken cancellationToken)
        {
            var query = _context.StudentCourses
                .AsNoTracking()
                .Where(sc => studentIds.Contains(sc.StudentId));

            if (scope.AllowedCourseIds is not null)
                query = query.Where(sc => scope.AllowedCourseIds.Contains(sc.CourseId));

            return await query
                .Select(sc => new EnrollmentListItem
                {
                    StudentId = sc.StudentId,
                    Type = EnrollmentProgressType.Course,
                    CourseId = sc.CourseId,
                    CourseName = sc.Course.Name,
                    SectionId = null,
                    SectionName = null,
                    EnrolledAt = sc.EnrolledAt,
                    WatchedCount = sc.NumberOfCourseVideosWatched,
                    TotalVideoCount = sc.Course.NumberOfVideos,
                    ProgressPercent = sc.Progress
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<EnrollmentListItem>> QuerySectionOnlyStubs(
            IReadOnlyList<Guid> studentIds,
            EnrollmentProgressScope scope,
            CancellationToken cancellationToken)
        {
            var query = _context.StudentSections
                .AsNoTracking()
                .Where(ss => studentIds.Contains(ss.StudentId))
                .Where(ss => !_context.StudentCourses.Any(sc =>
                    sc.StudentId == ss.StudentId &&
                    sc.CourseId == ss.Section.CourseId));

            if (scope.AllowedSectionIds is not null)
                query = query.Where(ss => scope.AllowedSectionIds.Contains(ss.SectionId));

            if (scope.AllowedCourseIds is not null)
                query = query.Where(ss => scope.AllowedCourseIds.Contains(ss.Section.CourseId));

            return await query
                .Select(ss => new EnrollmentListItem
                {
                    StudentId = ss.StudentId,
                    Type = EnrollmentProgressType.SectionOnly,
                    CourseId = ss.Section.CourseId,
                    CourseName = ss.Section.Course.Name,
                    SectionId = ss.SectionId,
                    SectionName = ss.Section.Name,
                    EnrolledAt = ss.EnrolledAt,
                    WatchedCount = ss.NumberOfSectionVideosWatched,
                    TotalVideoCount = ss.Section.NumberOfVideos,
                    ProgressPercent = ss.Progress
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<SubscribedSectionItem>> LoadSubscribedSectionsAsync(
            IReadOnlyList<Guid> studentIds,
            List<Guid> courseIds,
            EnrollmentProgressScope scope,
            CancellationToken cancellationToken)
        {
            if (courseIds.Count == 0)
                return [];

            var explicitSectionsQuery = _context.StudentSections
                .AsNoTracking()
                .Where(ss => studentIds.Contains(ss.StudentId) && courseIds.Contains(ss.Section.CourseId))
                .Where(ss => !_context.StudentCourses.Any(sc => sc.StudentId == ss.StudentId && sc.CourseId == ss.Section.CourseId));

            if (scope.AllowedSectionIds is not null)
                explicitSectionsQuery = explicitSectionsQuery.Where(ss => scope.AllowedSectionIds.Contains(ss.SectionId));

            var explicitSections = await explicitSectionsQuery
                .Select(ss => new SubscribedSectionItem
                {
                    StudentId = ss.StudentId,
                    SectionId = ss.SectionId,
                    CourseId = ss.Section.CourseId,
                    SectionName = ss.Section.Name,
                    WatchedCount = ss.NumberOfSectionVideosWatched,
                    TotalVideoCount = ss.Section.NumberOfVideos
                })
                .ToListAsync(cancellationToken);

            var fullCourseSectionsQuery = _context.StudentCourses
                .AsNoTracking()
                .Where(sc => studentIds.Contains(sc.StudentId) && courseIds.Contains(sc.CourseId))
                .SelectMany(sc => sc.Course!.Sections.Select(sec => new SubscribedSectionItem
                {
                    StudentId = sc.StudentId,
                    SectionId = sec.Id,
                    CourseId = sec.CourseId,
                    SectionName = sec.Name,
                    WatchedCount = sec.Videos.Count(v => v.StudentVideos.Any(sv => sv.StudentId == sc.StudentId && sv.Progress == 100)),
                    TotalVideoCount = sec.NumberOfVideos
                }));

            if (scope.AllowedSectionIds is not null)
                fullCourseSectionsQuery = fullCourseSectionsQuery.Where(s => scope.AllowedSectionIds.Contains(s.SectionId));

            var fullCourseSections = await fullCourseSectionsQuery.ToListAsync(cancellationToken);

            return explicitSections.Concat(fullCourseSections).ToList();
        }

        private async Task<List<ExamProgressRow>> LoadExamProgressRowsAsync(
            IReadOnlyList<Guid> studentIds,
            List<Guid> courseIds,
            CancellationToken cancellationToken)
        {
            if (courseIds.Count == 0 || studentIds.Count == 0)
                return [];

            var exams = await _context.Exams
                .AsNoTracking()
                .Where(e => courseIds.Contains(e.CourseId))
                .Select(e => new
                {
                    e.CourseId,
                    e.SectionId,
                    e.TotalMark,
                    Results = e.ExamResults
                        .Where(r => studentIds.Contains(r.StudentId))
                        .Select(r => new { r.StudentId, r.Status, r.StudentMark })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            var rows = new List<ExamProgressRow>();
            foreach (var exam in exams)
            {
                foreach (var studentId in studentIds)
                {
                    var result = exam.Results.FirstOrDefault(r => r.StudentId == studentId);
                    rows.Add(new ExamProgressRow
                    {
                        StudentId = studentId,
                        CourseId = exam.CourseId,
                        SectionId = exam.SectionId,
                        TotalMark = exam.TotalMark,
                        Status = result?.Status,
                        StudentMark = result?.StudentMark
                    });
                }
            }

            return rows;
        }

        private async Task<List<SheetProgressRow>> LoadSheetProgressRowsAsync(
            IReadOnlyList<Guid> studentIds,
            List<Guid> courseIds,
            List<Guid> sectionIds,
            CancellationToken cancellationToken)
        {
            if ((courseIds.Count == 0 && sectionIds.Count == 0) || studentIds.Count == 0)
                return [];

            var sheets = await _context.Sheets
                .AsNoTracking()
                .Where(s => s.Type == SheetType.QuestionSheet)
                .Where(s =>
                    (s.CourseId.HasValue && courseIds.Contains(s.CourseId.Value)) ||
                    (s.SectionId.HasValue && sectionIds.Contains(s.SectionId.Value)) ||
                    (s.SectionId.HasValue && courseIds.Contains(s.Section!.CourseId)) ||
                    (s.VideoId.HasValue && sectionIds.Contains(s.Video!.SectionId)) ||
                    (s.VideoId.HasValue && courseIds.Contains(s.Video!.Section!.CourseId)))
                .Select(s => new
                {
                    ParentCourseId = s.CourseId ?? (s.SectionId != null ? s.Section!.CourseId : s.Video!.Section!.CourseId),
                    ParentSectionId = s.SectionId ?? (s.VideoId != null ? s.Video!.SectionId : (Guid?)null),
                    s.DueDate,
                    Submissions = s.AnswersSheets
                        .Where(a => studentIds.Contains(a.StudentId))
                        .Select(a => new { a.StudentId, a.IsApproved, a.CreatedAt })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            var rows = new List<SheetProgressRow>();
            foreach (var sheet in sheets)
            {
                foreach (var studentId in studentIds)
                {
                    var submission = sheet.Submissions.FirstOrDefault(a => a.StudentId == studentId);
                    rows.Add(new SheetProgressRow
                    {
                        StudentId = studentId,
                        ParentCourseId = sheet.ParentCourseId,
                        ParentSectionId = sheet.ParentSectionId,
                        DueDate = sheet.DueDate,
                        HasSubmission = submission is not null,
                        IsApproved = submission?.IsApproved ?? false,
                        SubmittedAt = submission?.CreatedAt
                    });
                }
            }

            return rows;
        }

        public sealed class EnrollmentListItem
        {
            public Guid StudentId { get; init; }
            public EnrollmentProgressType Type { get; init; }
            public Guid CourseId { get; init; }
            public string CourseName { get; init; } = string.Empty;
            public Guid? SectionId { get; init; }
            public string? SectionName { get; init; }
            public DateTimeOffset EnrolledAt { get; init; }
            public int WatchedCount { get; init; }
            public int TotalVideoCount { get; init; }
            public decimal? ProgressPercent { get; init; }
        }

        internal sealed class SubscribedSectionItem
        {
            public Guid StudentId { get; init; }
            public Guid SectionId { get; init; }
            public Guid CourseId { get; init; }
            public string SectionName { get; init; } = string.Empty;
            public int WatchedCount { get; init; }
            public int TotalVideoCount { get; init; }
        }

        internal sealed class ExamProgressRow
        {
            public Guid StudentId { get; init; }
            public Guid CourseId { get; init; }
            public Guid? SectionId { get; init; }
            public decimal TotalMark { get; init; }
            public ExamResultStatus? Status { get; init; }
            public decimal? StudentMark { get; init; }
        }

        internal sealed class SheetProgressRow
        {
            public Guid StudentId { get; init; }
            public Guid ParentCourseId { get; init; }
            public Guid? ParentSectionId { get; init; }
            public DateTimeOffset? DueDate { get; init; }
            public bool HasSubmission { get; init; }
            public bool IsApproved { get; init; }
            public DateTimeOffset? SubmittedAt { get; init; }
        }

        public sealed class InstructorStudentSummary
        {
            public Guid StudentId { get; init; }
            public string StudentName { get; init; } = string.Empty;
            public string? StudentEmail { get; init; }
        }
    }
}
