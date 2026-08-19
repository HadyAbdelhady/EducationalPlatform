using Application.Features.Exams.DTOs;

namespace Application.Features.HomeScreen.DTOs
{
    public class InstructorStudentExamDto : ExamListDto
    {
        public Guid CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public Guid? SectionId { get; set; }

        public string? SectionName { get; set; }
    }
}
