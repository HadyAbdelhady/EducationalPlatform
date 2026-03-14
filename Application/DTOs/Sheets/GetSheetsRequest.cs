using Domain.enums;

namespace Application.DTOs.Sheets
{
    /// <summary>
    /// Request for the unified get-sheets endpoint.
    /// Bind from query: ?sheetType=TutorialSheet&targetType=Course&targetId={guid}
    /// </summary>
    public class GetSheetsRequest
    {
        public required SheetType SheetType { get; set; }
        public required SheetTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }
    }
}
