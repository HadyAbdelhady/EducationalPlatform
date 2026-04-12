namespace Application.DTOs.Sheets
{
    public class SheetResponse : SheetItem
    {
        public DateTimeOffset? DueDate { get; set; }
    }
}
