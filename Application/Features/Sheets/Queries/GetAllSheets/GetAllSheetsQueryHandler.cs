using Application.Common;
using Application.Features.Sheets.DTOs;
using Application.Common.Interfaces;
using Application.Features.Sheets.Interfaces;
using Application.Common;
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
            var skeleton = request.RequestSkeleton ?? new GetAllEntityRequestSkeleton();
            return await service.GetSheetsAsync(
                request.TargetId,
                request.SheetType,
                request.TargetType,
                skeleton,
                cancellationToken);
        }
    }
}

