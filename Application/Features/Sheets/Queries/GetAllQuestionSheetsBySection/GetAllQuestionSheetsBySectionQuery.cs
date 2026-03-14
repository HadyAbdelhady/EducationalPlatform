using Application.DTOs.Sheets;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sheets.Queries.GetAllQuestionSheetsBySection
{
    public class GetAllQuestionSheetsBySectionQuery : IRequest<Result<PaginatedResult<SheetResponse>>>
    {
        public Guid SectionId { get; set; }
    }
}



