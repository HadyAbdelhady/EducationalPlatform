namespace Application.DTOs.Courses
{
    public class CourseUpdateRequest
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid InstructorId { get; set; }
        public decimal Price { get; set; }
        public string PictureUrl { get; set; } = string.Empty;
        public string IntroVideoUrl { get; set; } = string.Empty;
    }
}
