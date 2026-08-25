namespace Application.Features.Sheets.DTOs
{
    public class StudentAnswersSheetSubmissionDto
    {
        public Guid AnswersSheetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SheetUrl { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentProfilePicture { get; set; } = string.Empty;
    }

    public class QuestionSheetWithSubmissionsDto : SheetResponse
    {
        public Guid? CourseId { get; set; }
        public string? CourseName { get; set; }
        public Guid? SectionId { get; set; }
        public string? SectionName { get; set; }
        public Guid? VideoId { get; set; }
        public string? VideoName { get; set; }
        public int SubmissionsCount { get; set; }
        public List<StudentAnswersSheetSubmissionDto> Submissions { get; set; } = [];
    }

    public class SubmittedAnswersSheetDto : StudentAnswersSheetSubmissionDto
    {
        public Guid QuestionsSheetId { get; set; }
        public string QuestionsSheetName { get; set; } = string.Empty;
        public string QuestionsSheetUrl { get; set; } = string.Empty;
        public DateTimeOffset? DueDate { get; set; }
        public Guid? CourseId { get; set; }
        public string? CourseName { get; set; }
        public Guid? SectionId { get; set; }
        public string? SectionName { get; set; }
        public Guid? VideoId { get; set; }
        public string? VideoName { get; set; }
    }
}
