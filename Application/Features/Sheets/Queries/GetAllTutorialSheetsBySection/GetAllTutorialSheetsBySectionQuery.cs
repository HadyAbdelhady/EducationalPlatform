using Application.DTOs.Sheets;
using Application.ResultWrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Sheets.Queries.GetAllTutorialSheetsBySection
{
    public class GetAllTutorialSheetsBySectionQuery : IRequest<Result<PaginatedResult<SheetResponse>>>
    {
        public Guid SectionId { get; set; }
    }
}



