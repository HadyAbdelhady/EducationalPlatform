using Application.DTOs.Sheet;
using Application.ResultWrapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.AnswersSheet.Commands.UpdateAnswersSheet
{
    public class UpdateAnswersSheetCommand : IRequest<Result<AnswersSheetUpdateResponse>>
    {
        public Guid AnswersSheetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public IFormFile SheetFile { get; set; } = null!;
    }
}


