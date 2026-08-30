namespace Application.Features.Sheets.DTOs
{
    public class LiveSheetResponse
    {
        public Guid SheetId { get; set; }
        public string SheetName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTimeOffset? DueDate { get; set; }
        public int EnrolledCount { get; set; }
        public int SubmittedCount { get; set; }
        public int NotSubmittedCount { get; set; }
        public List<LiveSheetStudentDto> Students { get; set; } = [];
    }

    public class LiveSheetStudentDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public bool HasSubmitted { get; set; }
        public bool? IsApproved { get; set; }
        public DateTimeOffset? SubmittedAt { get; set; }
    }
}
