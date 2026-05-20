namespace Application.DTOs.Sheets
{
    public class SheetItemResponse
    {
        public Guid BaseSheetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SheetUrl { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

    }
}
