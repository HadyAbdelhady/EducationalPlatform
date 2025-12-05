using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Videos
{
    public class VideoCreationResponse
    {
        public Guid VideoId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
    }
}
