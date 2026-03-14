namespace Application.DTOs.Courses
{
    public class CourseCreationResponse
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public Guid EducationYearId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
