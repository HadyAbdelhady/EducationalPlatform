using Application.DTOs.HomeScreen;
using Domain;
using Domain.enums;

namespace Infrastructure.Progress
{
    internal static class EnrollmentProgressMapper
    {
        public static EnrollmentProgressDto MapEnrollmentProgress(
            EnrollmentProgressHelper.EnrollmentListItem enrollment,
            IReadOnlyList<EnrollmentProgressHelper.ExamProgressRow> examRows,
            IReadOnlyList<EnrollmentProgressHelper.SheetProgressRow> sheetRows,
            IReadOnlyList<EnrollmentProgressHelper.SubscribedSectionItem> subscribedSections)
        {
            if (enrollment.Type == EnrollmentProgressType.SectionOnly)
            {
                var sectionId = enrollment.SectionId!.Value;
                return new EnrollmentProgressDto
                {
                    EnrollmentType = enrollment.Type.ToApiValue(),
                    CourseId = enrollment.CourseId,
                    CourseName = enrollment.CourseName,
                    SectionId = sectionId,
                    SectionName = enrollment.SectionName,
                    EnrolledAt = enrollment.EnrolledAt,
                    Videos = BuildVideoAggregate(enrollment.WatchedCount, enrollment.TotalVideoCount, enrollment.ProgressPercent),
                    Exams = BuildExamAggregate(examRows.Where(e => e.SectionId == sectionId)),
                    Sheets = BuildSheetAggregate(sheetRows.Where(s => s.ParentSectionId == sectionId)),
                    Sections = null
                };
            }

            var courseSections = subscribedSections
                .Where(s => s.CourseId == enrollment.CourseId)
                .Select(s => new SectionProgressAggregate
                {
                    SectionId = s.SectionId,
                    SectionName = s.SectionName,
                    Videos = BuildVideoAggregate(s.WatchedCount, s.TotalVideoCount, null),
                    Exams = BuildExamAggregate(examRows.Where(e => e.SectionId == s.SectionId)),
                    Sheets = BuildSheetAggregate(sheetRows.Where(sh => sh.ParentSectionId == s.SectionId))
                })
                .ToList();

            return new EnrollmentProgressDto
            {
                EnrollmentType = enrollment.Type.ToApiValue(),
                CourseId = enrollment.CourseId,
                CourseName = enrollment.CourseName,
                EnrolledAt = enrollment.EnrolledAt,
                Videos = BuildVideoAggregate(enrollment.WatchedCount, enrollment.TotalVideoCount, enrollment.ProgressPercent),
                Exams = BuildExamAggregate(examRows.Where(e => e.CourseId == enrollment.CourseId)),
                Sheets = BuildSheetAggregate(sheetRows.Where(s => s.ParentCourseId == enrollment.CourseId)),
                Sections = courseSections.Count > 0 ? courseSections : null
            };
        }

        public static VideoProgressAggregate BuildVideoAggregate(
            int watchedCount,
            int totalCount,
            decimal? storedProgress)
        {
            decimal? progressPercent = storedProgress;
            if (storedProgress is > 0 and <= 1)
                progressPercent = Math.Round(storedProgress.Value * 100m, 2);
            else if (!progressPercent.HasValue && totalCount > 0)
                progressPercent = Math.Round((decimal)watchedCount / totalCount * 100m, 2);

            return new VideoProgressAggregate
            {
                WatchedCount = watchedCount,
                TotalCount = totalCount,
                ProgressPercent = progressPercent
            };
        }

        public static ExamProgressAggregate BuildExamAggregate(IEnumerable<EnrollmentProgressHelper.ExamProgressRow> exams)
        {
            var examList = exams.ToList();
            var notStarted = 0;
            var inProgress = 0;
            var passed = 0;
            var failed = 0;
            var scorePercents = new List<decimal>();

            foreach (var exam in examList)
            {
                if (exam.Status is null or ExamResultStatus.NotStarted)
                {
                    notStarted++;
                    continue;
                }

                switch (exam.Status)
                {
                    case ExamResultStatus.InProgress:
                        inProgress++;
                        break;
                    case ExamResultStatus.Passed:
                        passed++;
                        break;
                    case ExamResultStatus.Failed:
                        failed++;
                        break;
                }

                if (exam.StudentMark.HasValue && exam.TotalMark > 0)
                    scorePercents.Add(exam.StudentMark.Value / exam.TotalMark * 100m);
            }

            return new ExamProgressAggregate
            {
                TotalCount = examList.Count,
                NotStartedCount = notStarted,
                InProgressCount = inProgress,
                PassedCount = passed,
                FailedCount = failed,
                AverageScorePercent = scorePercents.Count > 0 ? Math.Round(scorePercents.Average(), 2) : null,
                BestScorePercent = scorePercents.Count > 0 ? Math.Round(scorePercents.Max(), 2) : null
            };
        }

        public static SheetProgressAggregate BuildSheetAggregate(IEnumerable<EnrollmentProgressHelper.SheetProgressRow> sheets)
        {
            var sheetList = sheets.ToList();
            var now = EgyptTime.UtcNow;

            return new SheetProgressAggregate
            {
                TotalCount = sheetList.Count,
                NotSubmittedCount = sheetList.Count(s => !s.HasSubmission),
                SubmittedPendingCount = sheetList.Count(s => s.HasSubmission && !s.IsApproved),
                ApprovedCount = sheetList.Count(s => s.IsApproved),
                NextDueDate = sheetList
                    .Where(s => s.DueDate.HasValue && s.DueDate.Value >= now && !s.IsApproved)
                    .Select(s => s.DueDate)
                    .OrderBy(d => d)
                    .FirstOrDefault(),
                LastSubmittedAt = sheetList
                    .Where(s => s.SubmittedAt.HasValue)
                    .Select(s => s.SubmittedAt)
                    .OrderByDescending(d => d)
                    .FirstOrDefault()
            };
        }
    }
}
