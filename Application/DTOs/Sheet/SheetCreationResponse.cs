using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Sheet
{
    public class SheetCreationResponse
    {
        public Guid SheetId { get; set; }
        public string? SheetUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
