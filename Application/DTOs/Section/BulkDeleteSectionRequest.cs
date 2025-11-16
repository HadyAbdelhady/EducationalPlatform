using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Section
{
    public class BulkDeleteSectionRequest
    {
        public List<Guid> SectionIds { get; set; } = new();
    }
}
