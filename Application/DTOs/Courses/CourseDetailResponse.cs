namespace Application.DTOs.Courses
{
    public class CourseDetailResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? PictureUrl { get; set; }
        public string? IntroVideoUrl { get; set; }
        public bool IsEnrolled { get; set; }
        public int NumberOfVideos { get; set; }
        public decimal? Rating { get; set; }
        public int TotalReviews { get; set; }
        public int NumberOfStudents { get; set; }
        public int NumberOfSheets { get; set; }
        public int NumberOfWatchedVideos { get; set; }
        public int NumberOfSections { get; set; }
        public int ProgressPercentage { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public List<InstructorInfoDto> Instructors { get; set; } = [];
    }

    public class InstructorInfoDto
    {
        public Guid InstructorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PersonalPictureUrl { get; set; }
        public string? GmailExternal { get; set; }
    }
}
