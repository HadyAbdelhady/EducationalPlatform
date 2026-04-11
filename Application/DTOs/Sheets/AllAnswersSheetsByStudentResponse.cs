namespace Application.DTOs.Sheets
{
    public class AllAnswersSheetsByStudentResponse : SheetItem
    {
        public Guid QuestionsSheetId { get; set; }
        public string QuestionsSheetName { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
    }
}

