using Application.ResultWrapper;

namespace Application.DTOs.HomeScreen
{
    public class InstructorStudentsProgressResponse
    {
        public PaginatedResult<InstructorStudentProgressDto> Students { get; set; } = new();
    }

    public class InstructorStudentProgressDto
    {
        public Guid StudentId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string? StudentEmail { get; set; }

        public List<EnrollmentProgressDto> Enrollments { get; set; } = [];
    }
}
