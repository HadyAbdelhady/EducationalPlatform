using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Sheets
{
    public class SheetResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SheetUrl { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DueDate { get; set; }
    }
}
