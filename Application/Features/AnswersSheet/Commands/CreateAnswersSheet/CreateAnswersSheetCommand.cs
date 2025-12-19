using Application.DTOs.Sheets;
using Application.ResultWrapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.AnswersSheets.Commands.CreateAnswersSheet
{
    public class CreateAnswersSheetCommand : IRequest<Result<AnswersSheetCreationResponse>>
    {
        public string Name { get; set; } = string.Empty;
        public IFormFile SheetFile { get; set; } = null!;
        public Guid QuestionsSheetId { get; set; }
        public Guid StudentId { get; set; }
    }
}


