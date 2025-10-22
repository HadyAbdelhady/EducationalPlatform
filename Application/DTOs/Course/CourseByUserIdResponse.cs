namespace Application.DTOs.Course
{
    public class CourseByUserIdResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public int NumberOfStudents { get; set; }
        public int NumberOfVideos{ get; set; }
        public int NumberOfSections { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
