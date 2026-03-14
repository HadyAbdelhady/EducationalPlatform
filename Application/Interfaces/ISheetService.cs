using Application.DTOs.Sheets;
using Application.ResultWrapper;
using Domain.enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISheetService
    {
        Task<Result<PaginatedResult<SheetResponse>>> GetSheetsAsync(
            Guid targetId,
            SheetType sheetType,
            CancellationToken cancellationToken);
    }
}

