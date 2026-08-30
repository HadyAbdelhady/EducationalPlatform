using Application.Common;
using Application.Features.Sheets.DTOs;
using MediatR;

namespace Application.Features.Sheets.Queries.GetLiveSheet
{
    public class GetLiveSheetQuery : IRequest<Result<LiveSheetResponse>>
    {
        public Guid SheetId { get; set; }
        public Guid InstructorId { get; set; }
    }
}
