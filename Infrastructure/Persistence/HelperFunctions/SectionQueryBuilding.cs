using Application.Interfaces.BaseFilters;
using Domain.Entities;

namespace Infrastructure.Persistence.HelperFunctions
{
    public class SectionFilterRegistry : IBaseFilterRegistry<Section>
    {
        public Dictionary<string, Func<IQueryable<Section>, string, IQueryable<Section>>> Filters { get; } = new()
        {
            ["name"] = (q, value) => q.Where(p => p.Name == value),
            ["instructorid"] = (q, value) => q.Where(p => p.InstructorSections.Any(i => i.InstructorId == Guid.Parse(value))),
            ["rating"] = (q, value) => q.Where(p => p.Rating != null && p.Rating >= decimal.Parse(value)),
            ["studentid"] = (q, value) => q.Where(p => p.StudentSections.Any(s => s.StudentId == Guid.Parse(value)))
        };
       
        public Dictionary<string, Func<IQueryable<Section>, bool, IOrderedQueryable<Section>>> Sorts { get; } = new()
        {
            ["name"] = (q, desc) => desc ? q.OrderByDescending(s => s.Name) : q.OrderBy(s => s.Name),
            ["price"] = (q, desc) => desc ? q.OrderByDescending(s => s.Price) : q.OrderBy(s => s.Price),
            ["createdat"] = (q, desc) => desc ? q.OrderByDescending(s => s.CreatedAt) : q.OrderBy(s => s.CreatedAt)
        };
    }
}
