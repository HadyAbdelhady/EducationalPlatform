using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Sections
{
    public class BulkCreateSectionRequest
    {
        public List<CreateSectionRequest> Sections { get; set; } = new();
    }
}
