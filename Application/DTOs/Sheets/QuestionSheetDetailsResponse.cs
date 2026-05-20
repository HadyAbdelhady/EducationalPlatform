namespace Application.DTOs.Sheets
{
    public class QuestionSheetDetailsResponse : SheetItemResponse
    {
        public Guid? QuestionsSheetId { get; set; }
        public string? QuestionsSheetName { get; set; }
        public bool? IsApproved { get; set; }
        public DateTimeOffset? DueDate { get; set; }
    }

}
