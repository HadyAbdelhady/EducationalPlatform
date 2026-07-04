using Application.Common;
using Domain.enums;

namespace Application.Features.Sheets.DTOs
{
    public class GetSheetsRequest
    {
        public required SheetType SheetType { get; set; }
        public SheetTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }

        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
    }
}
