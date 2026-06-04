using Application.ResultWrapper;

namespace Application.DTOs.HomeScreen
{
    public class StudentEnrollmentProgressResponse
    {
        public ProgressGlobalSummary Global { get; set; } = new();

        public PaginatedResult<EnrollmentProgressDto> Enrollments { get; set; } = new();

        public PaginatedResult<UpcomingMilestoneDto> Milestones { get; set; } = new();
    }

    public class ProgressGlobalSummary
    {
        public int InProgressCoursesCount { get; set; }

        public int CompletedLessonsCount { get; set; }

        public decimal AverageGrade { get; set; }

        public string AverageGradeLetter { get; set; } = string.Empty;
    }

    public class EnrollmentProgressDto
    {
        public string EnrollmentType { get; set; } = string.Empty;

        public Guid CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public Guid? SectionId { get; set; }

        public string? SectionName { get; set; }

        public DateTimeOffset EnrolledAt { get; set; }

        public VideoProgressAggregate Videos { get; set; } = new();

        public ExamProgressAggregate Exams { get; set; } = new();

        public SheetProgressAggregate Sheets { get; set; } = new();

        public List<SectionProgressAggregate>? Sections { get; set; }
    }

    public class SectionProgressAggregate
    {
        public Guid SectionId { get; set; }

        public string SectionName { get; set; } = string.Empty;

        public VideoProgressAggregate Videos { get; set; } = new();

        public ExamProgressAggregate Exams { get; set; } = new();

        public SheetProgressAggregate Sheets { get; set; } = new();
    }

    public class VideoProgressAggregate
    {
        public int WatchedCount { get; set; }

        public int TotalCount { get; set; }

        public decimal? ProgressPercent { get; set; }
    }

    public class ExamProgressAggregate
    {
        public int TotalCount { get; set; }

        public int NotStartedCount { get; set; }

        public int InProgressCount { get; set; }

        public int PassedCount { get; set; }

        public int FailedCount { get; set; }

        public decimal? AverageScorePercent { get; set; }

        public decimal? BestScorePercent { get; set; }
    }

    public class SheetProgressAggregate
    {
        public int TotalCount { get; set; }

        public int NotSubmittedCount { get; set; }

        public int SubmittedPendingCount { get; set; }

        public int ApprovedCount { get; set; }

        public DateTimeOffset? NextDueDate { get; set; }

        public DateTimeOffset? LastSubmittedAt { get; set; }
    }

    public class UpcomingMilestoneDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        /// <summary>
        /// Either "Exam" or "Sheet".
        /// </summary>
        public string Type { get; set; } = string.Empty;

        public DateTimeOffset DueAt { get; set; }
    }
}
