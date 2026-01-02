namespace Application.DTOs.HomeScreen
{
    public class StudentHomeScreenResponse
    {
        public List<EnrolledCourseDto> Courses { get; set; } = [];
        public List<LatestVideoDto> Videos { get; set; } = [];
        public List<StudentExamDto> Exams { get; set; } = [];
        public List<StudentSheetDto> Sheets { get; set; } = [];
    }

    public class EnrolledCourseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        public int NumberOfVideos { get; set; }
        public int NumberOfExams { get; set; }
        public int NumberOfSheets { get; set; }
        public decimal? Rating { get; set; }
    }

    public class LatestVideoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class StudentExamDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public decimal TotalMark { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public int? DurationInMinutes { get; set; }
    }

    public class StudentSheetDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
}

