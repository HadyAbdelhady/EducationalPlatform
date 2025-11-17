using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Section
{
    public class BulkCreateSectionRequest
    {
        public List<CreateSectionRequest> Sections { get; set; } = new();
    }
}
