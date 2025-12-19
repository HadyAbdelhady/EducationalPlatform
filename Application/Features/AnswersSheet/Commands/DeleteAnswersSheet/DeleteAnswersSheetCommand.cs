using Application.ResultWrapper;
using MediatR;

namespace Application.Features.AnswersSheets.Commands.DeleteAnswersSheet
{
    public class DeleteAnswersSheetCommand : IRequest<Result<string>>
    {
        public Guid AnswersSheetId { get; set; }
    }
}


