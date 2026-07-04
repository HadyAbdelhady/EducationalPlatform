using Application.Common;
using Application.Features.Sheets.DTOs;
using Application.Common;
using Domain.enums;

namespace Application.Features.Sheets.Interfaces
{
    public interface ISheetService
    {
        Task<Result<PaginatedResult<SheetResponse>>> GetSheetsAsync(
            Guid targetId,
            SheetType sheetType,
            SheetTargetType targetType,
            GetAllEntityRequestSkeleton requestSkeleton,
            CancellationToken cancellationToken);
    }
}

