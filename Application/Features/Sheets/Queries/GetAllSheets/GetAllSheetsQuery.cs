using Application.DTOs;
using Application.DTOs.Sheets;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Sheets.Queries.GetAllSheets
{
    public class GetAllSheetsQuery : IRequest<Result<PaginatedResult<SheetResponse>>>
    {
        public required SheetType SheetType { get; init; }
        public required SheetTargetType TargetType { get; init; }
        public required Guid TargetId { get; init; }
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
    }
}

