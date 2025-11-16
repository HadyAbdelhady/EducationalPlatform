using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Section
{
    public class CreateSectionRequest
    {
        public class SectionCreationRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
            public decimal Price { get; set; }
            public Guid CourseId { get; set; }
        }
    }
}
