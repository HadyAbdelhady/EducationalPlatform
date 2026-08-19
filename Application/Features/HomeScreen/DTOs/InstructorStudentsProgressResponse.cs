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

        public string PhoneNumber { get; set; } = string.Empty;

        public string ParentPhoneNumber { get; set; } = string.Empty;

        public string? LocationMaps { get; set; }

        public string Gender { get; set; } = string.Empty;

        public Guid EducationYearId { get; set; }

        public StudentOverallProgressDto Overall { get; set; } = new();
    }

    public class StudentOverallProgressDto
    {
        public VideoProgressAggregate Videos { get; set; } = new();

        public ExamProgressAggregate Exams { get; set; } = new();

        public SheetProgressAggregate Sheets { get; set; } = new();
    }

    public class InstructorStudentEnrollmentsResponse
    {
        public Guid StudentId { get; set; }

        public List<EnrollmentProgressDto> Enrollments { get; set; } = [];

        public List<StudentContentReviewDto> Reviews { get; set; } = [];
    }

    public class StudentContentReviewDto
    {
        public Guid Id { get; set; }

        public string Type { get; set; } = string.Empty;

        public Guid EntityId { get; set; }

        public string EntityName { get; set; } = string.Empty;

        public decimal StarRating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; }
    }
}
