using System.ComponentModel.DataAnnotations;

namespace Application.Features.Sections.DTOs
{
    public class BulkCreateSectionRequest
    {
        public Guid CourseId { get; set; }
        public List<BulkSectionDataForCreation> Sections { get; set; } = [];
    }
    public class BulkSectionDataForCreation
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }
}
