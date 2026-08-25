using Application.Features.HomeScreen.DTOs;
using Application.Features.Reviews.DTOs;

namespace Application.Features.Profiles.DTOs
{
    public class SharedEnrollmentDto
    {
        public Guid? CourseId { get; set; }
        public string? CourseName { get; set; }
        public Guid? SectionId { get; set; }
        public string? SectionName { get; set; }
        public bool IsEnrolled { get; set; }
        public DateTimeOffset? EnrolledAt { get; set; }

        public string? Description { get; set; }
        public string? PictureUrl { get; set; }
        public decimal? Price { get; set; }
        public int NumberOfVideos { get; set; }
        public int NumberOfSections { get; set; }
        public int NumberOfSheets { get; set; }
        public int NumberOfExams { get; set; }
        public int NumberOfStudents { get; set; }
        public decimal? Rating { get; set; }
    }

    public class StudentProfileForInstructorResponse
    {
        public Guid StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PersonalPictureUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid EducationYearId { get; set; }
        public string EducationYearName { get; set; } = string.Empty;
        public Guid? CenterId { get; set; }
        public string? CenterName { get; set; }
        public List<EnrollmentProgressDto> Enrollments { get; set; } = [];
    }

    public class InstructorProfileForStudentResponse
    {
        public Guid InstructorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PersonalPictureUrl { get; set; }
        public string? GmailExternal { get; set; }
        public decimal? Rating { get; set; }
        public List<SharedEnrollmentDto> SharedContent { get; set; } = [];
        public List<GetAllReviewsResponse> Reviews { get; set; } = [];
    }
}
