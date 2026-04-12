namespace Application.DTOs.Sheets
{
    public abstract class SheetItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SheetUrl { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DueDate { get; set; }

    }
}
