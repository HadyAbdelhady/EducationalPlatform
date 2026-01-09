using Application.Interfaces.BaseFilters;
using Domain.Entities;

namespace Infrastructure.Persistence.HelperFunctions
{
    public class CourseFilterRegistry : IBaseFilterRegistry<Course>
    {
        public Dictionary<string, Func<IQueryable<Course>, string, IQueryable<Course>>> Filters { get; } = new()
        {
            ["name"] = (q, value) => q.Where(p => p.Name == value),
            ["instructorid"] = (q, value) => q.Where(p => p.InstructorCourses.Any(i => i.InstructorId == Guid.Parse(value))),
            ["rating"] = (q, value) => q.Where(p => p.Rating != null && p.Rating >= decimal.Parse(value))
        };
       
        public Dictionary<string, Func<IQueryable<Course>, bool, IOrderedQueryable<Course>>> Sorts { get; } = new()
        {
            ["name"] = (q, desc) => desc ? q.OrderByDescending(c => c.Name) : q.OrderBy(c => c.Name),
            ["price"] = (q, desc) => desc ? q.OrderByDescending(c => c.Price) : q.OrderBy(c => c.Price),
            ["createdat"] = (q, desc) => desc ? q.OrderByDescending(c => c.CreatedAt) : q.OrderBy(c => c.CreatedAt)
        };
    }
}
