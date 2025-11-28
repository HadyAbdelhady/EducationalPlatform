using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Videos
{
    public class VideoByUserIdResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public DateTimeOffset DateOfCreation { get; set; }
        public string? Description { get; set; }
        public int NumberOfSheets { get; set; }
        public Guid? SectionId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
