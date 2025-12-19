using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Sheet
{
    public class AnswersSheetCreationRequest
    {
        public string Name { get; set; } = string.Empty;
        public IFormFile SheetFile { get; set; } = null!;
        public Guid QuestionsSheetId { get; set; }
        public Guid StudentId { get; set; }
    }
}


