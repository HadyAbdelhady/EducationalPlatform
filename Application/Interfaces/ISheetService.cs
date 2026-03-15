using Application.ResultWrapper;
using Domain.enums;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISheetService
    {
        Task<Result<PaginatedResult<object>>> GetSheetsAsync(
            Guid targetId,
            SheetType sheetType,
            CancellationToken cancellationToken);
    }
}

