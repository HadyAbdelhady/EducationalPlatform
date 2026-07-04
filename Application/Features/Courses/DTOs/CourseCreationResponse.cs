namespace Application.Features.Courses.DTOs
{
    public class CourseCreationResponse
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public Guid EducationYearId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
