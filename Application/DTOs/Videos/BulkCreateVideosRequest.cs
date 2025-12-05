using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Videos
{
    public class BulkCreateVideosRequest
    {
        public List<VideoCreationRequest> Videos { get; set; } = new();
    }
}
