using Domain.enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Sheets
{
    public class SheetCreationRequest
    {
        public string Name { get; set; } = string.Empty;
        public IFormFile SheetFile { get; set; } = null!;
        public SheetType Type { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? VideoId { get; set; }
        public Guid InstructorId { get; set; }
    }
}
