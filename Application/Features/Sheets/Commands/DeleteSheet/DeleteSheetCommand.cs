using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sheets.Commands.DeleteSheet
{
    public class DeleteSheetCommand : IRequest<Result<string>>
    {
        public Guid SheetId { get; set; }
    }
}
