using Application.DTOs.Sheets;
using Application.Interfaces;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sheets.Queries.GetAllSheets
{
    public class GetAllSheetsQueryHandler(ISheetServiceFactory sheetServiceFactory)
        : IRequestHandler<GetAllSheetsQuery, Result<PaginatedResult<SheetResponse>>>
    {
        private readonly ISheetServiceFactory _sheetServiceFactory = sheetServiceFactory;

        public async Task<Result<PaginatedResult<SheetResponse>>> Handle(
            GetAllSheetsQuery request,
            CancellationToken cancellationToken)
        {
            var service = _sheetServiceFactory.GetSheetService(request.TargetType);
            var pageNumber = request.RequestSkeleton?.PageNumber ?? 1;
            return await service.GetSheetsAsync(request.TargetId, request.SheetType, pageNumber, cancellationToken);
        }
    }
}

