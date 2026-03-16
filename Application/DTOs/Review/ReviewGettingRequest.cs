using Domain.enums;

namespace Application.DTOs.Review
{
    public record ReviewGettingRequest(
        Guid EntityId,
        ReviewEntityType EntityType,
        Dictionary<string, string> Filters,
        string SortBy,
        bool IsDescending = false);
}
