using Domain.enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Review
{
    public class ReviewCreationRequest
    {
        public Guid StudentId { get; set; }
        public Guid EntityId { get; set; }
        public required ReviewEntityType EntityType { get; set; }

        [Range(1, 5, ErrorMessage = "Rate Must be between 1 and 5.")]
        public int StarRating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
