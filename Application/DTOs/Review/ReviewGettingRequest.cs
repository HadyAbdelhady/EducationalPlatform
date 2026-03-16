using Domain.enums;

namespace Application.DTOs.Review
{
    public class ReviewGettingRequest
    {
        public GetAllEntityRequestSkeleton GetAllEntityRequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid EntityId { get; set; }
        public ReviewEntityType EntityType { get; set; }
    }

}
