namespace Application.DTOs.Sheet
{
    public class AnswersSheetUpdateResponse
    {
        public Guid AnswersSheetId { get; set; }
        public string? SheetUrl { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}


