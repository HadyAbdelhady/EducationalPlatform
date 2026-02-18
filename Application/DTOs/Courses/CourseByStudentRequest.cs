namespace Application.DTOs.Courses
{
    public class CourseByStudentRequest
    {
        public Guid StudentId { get; set; }
        public Dictionary<string, string> Filters { get; set; } = [];

        public string? SortBy { get; set; } = null;

        public bool IsDescending { get; set; } = false;
    }
}
