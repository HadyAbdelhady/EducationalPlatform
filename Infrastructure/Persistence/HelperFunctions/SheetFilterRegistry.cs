using Application.Interfaces.BaseFilters;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.HelperFunctions
{
    public class SheetFilterRegistry : IBaseFilterRegistry<Sheet>
    {
        public Dictionary<string, Func<IQueryable<Sheet>, string, IQueryable<Sheet>>> Filters { get; } = new()
        {
            ["courseid"] = (q, value) => q.Where(sh => sh.CourseId == Guid.Parse(value)),
            ["sectionid"] = (q, value) => q.Where(sh => sh.SectionId == Guid.Parse(value)),
            ["videoid"] = (q, value) => q.Where(sh => sh.VideoId == Guid.Parse(value)),
            ["instructorid"] = (q, value) => q.Where(sh => sh.InstructorId == Guid.Parse(value)),
            ["name"] = (q, value) => q.Where(sh => sh.Name.Contains(value)),
            ["sheetstatus"] = (q, value) =>
                                              q.Where(sh => sh.AnswersSheets.Any(a => a.IsApproved.Equals(value)))
                                              .Include(sh => sh.AnswersSheets.Where(a => a.IsApproved.Equals(value))),
        };

        public Dictionary<string, Func<IQueryable<Sheet>, bool, IOrderedQueryable<Sheet>>> Sorts { get; } = new()
        {
            ["name"] = (q, desc) => desc ? q.OrderByDescending(sh => sh.Name) : q.OrderBy(sh => sh.Name),
            ["createdat"] = (q, desc) => desc ? q.OrderByDescending(sh => sh.CreatedAt) : q.OrderBy(sh => sh.CreatedAt),
            ["duedate"] = (q, desc) => desc ? q.OrderByDescending(sh => sh.DueDate) : q.OrderBy(sh => sh.DueDate),
        };
    }
}
