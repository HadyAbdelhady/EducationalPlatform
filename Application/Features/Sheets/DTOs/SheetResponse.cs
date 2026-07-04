namespace Application.Features.Sheets.DTOs
{
    public class SheetResponse : SheetItem
    {
        public DateTimeOffset? DueDate { get; set; }

        public Guid? QuestionsSheetId { get; set; }
        public string? QuestionsSheetName { get; set; }
        public bool? IsApproved { get; set; }
    }
}
