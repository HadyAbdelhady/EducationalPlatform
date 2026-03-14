using Application.DTOs.Sheets;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sheets.Queries.GetAllQuestionSheetsByVideo
{
    public class GetAllQuestionSheetsByVideoQuery : IRequest<Result<PaginatedResult<SheetResponse>>>
    {
        public Guid VideoId { get; set; }
    }
}



