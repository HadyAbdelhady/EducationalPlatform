namespace Application.DTOs.HomeScreen
{
    public class InstructorDashboardResponse
    {
        public List<InstructorCourseDto> Courses { get; set; } = [];
        public List<InstructorStatsDto> Stats { get; set; } = [];
        public List<RecentActivityDto> RecentActivities { get; set; } = [];
        public List<PendingTaskDto> PendingTasks { get; set; } = [];
        public List<UpcomingExamDto> UpcomingExams { get; set; } = [];
        public List<UpcomingSheetDto> UpcomingSheets { get; set; } = [];
        public DateTimeOffset CurrentTime { get; set; }
    }

    public class InstructorCourseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        public int NumberOfStudents { get; set; }
        public decimal? Rating { get; set; }
        public decimal? Revenue { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int NumberOfVideos { get; set; }
        public int NumberOfExams { get; set; }
        public int NumberOfSheets { get; set; }
    }

    public class InstructorStatsDto
    {
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalVideos { get; set; }
        public int TotalExams { get; set; }
        public int TotalSheets { get; set; }
    }

    public class RecentActivityDto
    {
        public string ActivityType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; }
        public Guid? RelatedEntityId { get; set; }
    }

    public class PendingTaskDto
    {
        public string TaskType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTimeOffset? DueDate { get; set; }
        public int Priority { get; set; }
        public Guid? RelatedEntityId { get; set; }
    }

    public class UpcomingExamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTimeOffset StartTime { get; set; }
        public int DurationInMinutes { get; set; }
        public int NumberOfQuestions { get; set; }
        public string Status { get; set; } = string.Empty; // Draft, Published, Grading, Completed
        public int NumberOfEnrolledStudents { get; set; }
        public bool IsToday => StartTime.Date == DateTimeOffset.UtcNow.Date;
        public bool IsUrgent => StartTime <= DateTimeOffset.UtcNow.AddDays(3);
    }

    public class UpcomingSheetDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTimeOffset? DueDate { get; set; }
        public string Status { get; set; } = string.Empty; // Draft, Published, Grading, Completed
        public int NumberOfSubmittedStudents { get; set; }
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTimeOffset.UtcNow;
        public bool IsDueSoon => DueDate.HasValue && DueDate.Value <= DateTimeOffset.UtcNow.AddDays(2);
    }
}
