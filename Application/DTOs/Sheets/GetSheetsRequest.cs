using Domain.enums;

namespace Application.DTOs.Sheets
{
    public class GetSheetsRequest
    {
        public required SheetType SheetType { get; set; }
        public SheetTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }
    }
}
