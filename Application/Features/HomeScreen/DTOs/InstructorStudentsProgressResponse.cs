using Application.Common;

namespace Application.Features.HomeScreen.DTOs
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

        public string? StudentPictureUrl { get; set; }

        public StudentOverallProgressDto Overall { get; set; } = new();

        public List<EnrollmentProgressDto> Enrollments { get; set; } = [];
    }

    public class StudentOverallProgressDto
    {
        public VideoProgressAggregate Videos { get; set; } = new();

        public ExamProgressAggregate Exams { get; set; } = new();

        public SheetProgressAggregate Sheets { get; set; } = new();
    }
}
