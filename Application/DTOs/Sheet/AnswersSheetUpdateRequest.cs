using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Sheet
{
    public class AnswersSheetUpdateRequest
    {
        public Guid AnswersSheetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public IFormFile SheetFile { get; set; } = null!;
    }
}


