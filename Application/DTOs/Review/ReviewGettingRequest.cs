using Domain.enums;

namespace Application.DTOs.Review
{
    public record ReviewGettingRequest(
        Guid EntityId, 
        ReviewEntityType EntityType,
        Dictionary<string, string>? Filters = null,
        string? SortBy = null,
        bool IsDescending = false);
}
