namespace Application.DTOs.Sheets
{
    public class AllAnswersSheetsByStudentResponse
    {
        public Guid AnswersSheetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SheetUrl { get; set; } = null!;
        public Guid QuestionsSheetId { get; set; }
        public string QuestionsSheetName { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}

