using Application.DTOs.Sheets;
using Application.ResultWrapper;
using Domain.enums;

namespace Edu_Base.Application.Interfaces
{
    public interface ISheetService
    {
        Task<Result<PaginatedResult<SheetItem>>> GetSheetsAsync(
            Guid targetId,
            SheetType sheetType,
            int pageNumber,
            CancellationToken cancellationToken);
    }
}