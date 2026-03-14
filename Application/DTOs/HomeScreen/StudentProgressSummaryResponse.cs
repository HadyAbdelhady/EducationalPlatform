using Application.ResultWrapper;

namespace Application.DTOs.HomeScreen
{
    public class StudentProgressSummaryResponse
    {
        public int InProgressCoursesCount { get; set; }

        public int CompletedLessonsCount { get; set; }

        public decimal AverageGrade { get; set; }

        public string AverageGradeLetter { get; set; } = string.Empty;

        public PaginatedResult<StudentProgressCourseDto> Courses { get; set; } = new();

        public PaginatedResult<UpcomingMilestoneDto> UpcomingMilestones { get; set; } = new();
    }

    public class StudentProgressCourseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? PictureUrl { get; set; }

        public int CompletedLessons { get; set; }

        public int TotalLessons { get; set; }

        public decimal? GradePercent { get; set; }
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

