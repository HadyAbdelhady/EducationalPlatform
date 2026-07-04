using Application.Common;
using Domain.enums;

namespace Application.Features.Reviews.DTOs
{
    public class ReviewGettingRequest
    {
        public GetAllEntityRequestSkeleton GetAllEntityRequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid EntityId { get; set; }
        public ReviewEntityType EntityType { get; set; }
    }

}
