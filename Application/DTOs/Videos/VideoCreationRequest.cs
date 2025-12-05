using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Videos
{
    public class VideoCreationRequest
    {
        public string Name { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public DateTimeOffset DateOfCreation { get; set; }
        public string? Description { get; set; }
        public Guid? SectionId { get; set; }
    }
}
