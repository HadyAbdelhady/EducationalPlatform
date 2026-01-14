using Domain.enums;

namespace Application.DTOs.Review
{
    public class GetReviewByIdRequest
    {
        public Guid reviewId { get; set; }
        public ReviewEntityType EntityType { get; set; }
    }
}
