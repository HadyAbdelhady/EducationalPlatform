using Application.ResultWrapper;
using MediatR;

namespace Application.Features.AnswersSheets.Commands.ApproveAnswersSheet
{
    public class ApproveAnswersSheetCommand : IRequest<Result<string>>
    {
        public Guid AnswersSheetId { get; set; }
        public Guid InstructorId { get; set; }
    }
}
