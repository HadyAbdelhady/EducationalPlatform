namespace Application.DTOs.Courses
{
    public class CourseByStudentRequest
    {
        public Guid StudentId { get; set; }
        public bool FirstThreeCoursesOnly { get; set; }
    }
}
