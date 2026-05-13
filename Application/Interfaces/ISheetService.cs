using Application.DTOs;
using Application.DTOs.Sheets;
using Application.ResultWrapper;
using Domain.enums;

namespace Application.Interfaces
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

