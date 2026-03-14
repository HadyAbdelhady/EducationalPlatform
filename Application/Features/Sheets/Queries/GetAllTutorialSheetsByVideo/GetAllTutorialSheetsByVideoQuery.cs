using Application.DTOs.Sheets;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sheets.Queries.GetAllTutorialSheetsByVideo
{
    public class GetAllTutorialSheetsByVideoQuery : IRequest<Result<PaginatedResult<SheetResponse>>>
    {
        public Guid VideoId { get; set; }
    }
}



