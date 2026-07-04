using Microsoft.AspNetCore.Http;

namespace Application.Features.Sheets.DTOs
{
    public class AnswersSheetCreationRequest
    {
        public string Name { get; set; } = string.Empty;
        public IFormFile SheetFile { get; set; } = null!;
        public Guid QuestionsSheetId { get; set; }
    }
}


