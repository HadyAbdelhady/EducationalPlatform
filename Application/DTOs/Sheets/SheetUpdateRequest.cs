using Domain.enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Sheets
{
    public class SheetUpdateRequest
    {
        public Guid SheetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public IFormFile? SheetFile { get; set; } = null!;
        public DateTime? DueDate { get; set; }
    }
}
