namespace Application.Features.Sheets.DTOs
{
    public class AllAnswersSheetsByStudentResponse : SheetItem
    {
        public Guid QuestionsSheetId { get; set; }
        public string QuestionsSheetName { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
    }
}

