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
        public int NumberOfVideos { get; set; }
        public decimal? Rating { get; set; }
        public int TotalReviews { get; set; }
        public int NumberOfStudents { get; set; }
        public int NumberOfSheets { get; set; }

        public int NumberOfSections { get; set; }
        public DateTimeOffset DateOfCreation { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        // Instructor Information
        public List<InstructorInfoDto> Instructors { get; set; } = [];

        // Sections with detailed information
        public List<SectionDetailDto> Sections { get; set; } = [];

        // Reviews
        public List<CourseReviewDto> Reviews { get; set; } = [];
    }

    public class InstructorInfoDto
    {
        public Guid InstructorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PersonalPictureUrl { get; set; }
        public string? GmailExternal { get; set; }
    }

    public class SectionDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int NumberOfVideos { get; set; }
        public decimal? Rating { get; set; }
        public decimal? Price { get; set; }
        public DateTimeOffset DateOfCreation { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
    }

    public class CourseReviewDto
    {
        public Guid Id { get; set; }
        public int StarRating { get; set; }
        public string? Comment { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public StudentReviewInfoDto Student { get; set; } = null!;
    }

    public class StudentReviewInfoDto
    {
        public Guid StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PersonalPictureUrl { get; set; }
    }
}
