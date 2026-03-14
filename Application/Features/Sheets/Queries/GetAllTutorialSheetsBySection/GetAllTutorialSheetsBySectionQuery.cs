using Application.DTOs.Sheets;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sheets.Queries.GetAllTutorialSheetsBySection
{
    public class GetAllTutorialSheetsBySectionQuery : IRequest<Result<PaginatedResult<SheetResponse>>>
    {
        public Guid SectionId { get; set; }
    }
}



