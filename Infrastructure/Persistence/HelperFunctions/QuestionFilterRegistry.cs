using Application.Interfaces.BaseFilters;
using Domain.Entities;

namespace Infrastructure.Persistence.HelperFunctions
{
    public class QuestionFilterRegistry : IBaseFilterRegistry<Question>
    {
        public Dictionary<string, Func<IQueryable<Question>, string, IQueryable<Question>>> Filters { get; } = new()
        {
            ["courseid"] = (q, value) => q.Where(c => c.CourseId == Guid.Parse(value)),
            ["sectionid"] = (q, value) => q.Where(p => p.SectionId == Guid.Parse(value)),
        };

        public Dictionary<string, Func<IQueryable<Question>, bool, IOrderedQueryable<Question>>> Sorts { get; } = new()
        {
            ["name"] = (q, desc) => desc ? q.OrderByDescending(c => c.QuestionString) : q.OrderBy(c => c.QuestionString),
            ["createdat"] = (q, desc) => desc ? q.OrderByDescending(c => c.CreatedAt) : q.OrderBy(c => c.CreatedAt)
        };
    }
}
