using Application.Interfaces.BaseFilters;
using Domain.Entities;

namespace Infrastructure.Persistence.HelperFunctions
{
    public class VideoFilterRegistry : IBaseFilterRegistry<Video>
    {
        public Dictionary<string, Func<IQueryable<Video>, string, IQueryable<Video>>> Filters { get; } = new()
        {
            ["name"] = (q, value) => q.Where(v => v.Name.Contains(value)),
            ["sectionid"] = (q, value) => q.Where(v => v.SectionId == Guid.Parse(value)),
        };

        public Dictionary<string, Func<IQueryable<Video>, bool, IOrderedQueryable<Video>>> Sorts { get; } = new()
        {
            ["name"] = (q, desc) => desc ? q.OrderByDescending(v => v.Name) : q.OrderBy(v => v.Name),
            ["createdat"] = (q, desc) => desc ? q.OrderByDescending(v => v.CreatedAt) : q.OrderBy(v => v.CreatedAt),
        };
    }
}
