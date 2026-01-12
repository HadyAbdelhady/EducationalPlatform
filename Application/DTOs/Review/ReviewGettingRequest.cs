using Domain.enums;

namespace Application.DTOs.Review
{
    public record ReviewGettingRequest(Guid EntityId, ReviewEntityType EntityType);
}
