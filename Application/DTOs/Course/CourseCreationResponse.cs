namespace Application.DTOs.Course
{
    public class CourseCreationResponse
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
