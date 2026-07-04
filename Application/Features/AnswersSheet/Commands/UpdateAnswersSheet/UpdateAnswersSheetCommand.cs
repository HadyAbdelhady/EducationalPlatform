using Application.Features.Sheets.DTOs;
using Application.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.AnswersSheets.Commands.UpdateAnswersSheet
{
    public class UpdateAnswersSheetCommand : IRequest<Result<AnswersSheetUpdateResponse>>
    {
        public Guid AnswersSheetId { get; set; }
        public Guid StudentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public IFormFile SheetFile { get; set; } = null!;
    }
}





