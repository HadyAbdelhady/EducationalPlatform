using Application.DTOs.Sheets;
using Application.Features.Sheets.Queries.GetAllSheets;
using Application.Interfaces;
using Application.ResultWrapper;
using MediatR;

namespace Edu_Base.Application.Features.Sheets.Queries.GetAllSheets
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
            var pageNumber = request.RequestSkeleton?.PageNumber ?? 1;
            return await service.GetSheetsAsync(request.TargetId, request.SheetType, pageNumber, cancellationToken);
        }
    }
}       