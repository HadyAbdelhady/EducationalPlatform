namespace Application.DTOs.Sheet
{
    public class AnswersSheetCreationResponse
    {
        public Guid AnswersSheetId { get; set; }
        public string? SheetUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}


