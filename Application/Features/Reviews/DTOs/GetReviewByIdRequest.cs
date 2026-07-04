using Domain.enums;

namespace Application.Features.Reviews.DTOs
{
    public class GetReviewByIdRequest
    {
        public Guid reviewId { get; set; }
        public ReviewEntityType EntityType { get; set; }
    }
}
