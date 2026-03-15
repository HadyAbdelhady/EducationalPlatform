using Application.DTOs.Sheets;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Sheets.Queries.GetAllSheets
{
    public class GetAllSheetsQueryHandler(ISheetServiceFactory sheetServiceFactory)
        : IRequestHandler<GetAllSheetsQuery, Result<PaginatedResult<SheetItem>>>
    {
        private readonly ISheetServiceFactory _sheetServiceFactory = sheetServiceFactory;

        public async Task<Result<PaginatedResult<SheetItem>>> Handle(
            GetAllSheetsQuery request,
            CancellationToken cancellationToken)
        {
            var service = _sheetServiceFactory.GetSheetService(request.TargetType);
            return await service.GetSheetsAsync(request.TargetId, request.SheetType, cancellationToken);
        }
    }
}

