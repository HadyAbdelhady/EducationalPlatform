using Application.Interfaces.BaseFilters;
using Domain.Entities;

namespace Infrastructure.Persistence.HelperFunctions
{
    public class ExamFilterRegistry : IBaseFilterRegistry<Exam>
    {
        public Dictionary<string, Func<IQueryable<Exam>, string, IQueryable<Exam>>> Filters { get; } = new()
        {
            ["courseid"] = (q, value) => q.Where(e => e.CourseId == Guid.Parse(value)),
            ["sectionid"] = (q, value) => q.Where(e => e.SectionId == Guid.Parse(value)),
            ["examstatus"] = (q, value) => q.Where(e => e.Status.ToString().Equals(value, StringComparison.OrdinalIgnoreCase)),
            ["examtype"] = (q, value) => q.Where(e => e.ExamType.ToString().Equals(value, StringComparison.OrdinalIgnoreCase)),
            ["name"] = (q, value) => q.Where(e => e.Name.Contains(value, StringComparison.OrdinalIgnoreCase)),
            ["israndomized"] = (q, value) => q.Where(e => e.IsRandomized == bool.Parse(value)),
            ["starttime"] = (q, value) => q.Where(e => e.StartTime >= DateTimeOffset.Parse(value)),
            ["endtime"] = (q, value) => q.Where(e => e.EndTime <= DateTimeOffset.Parse(value)),
        };

        public Dictionary<string, Func<IQueryable<Exam>, bool, IOrderedQueryable<Exam>>> Sorts { get; } = new()
        {
            ["name"] = (q, desc) => desc ? q.OrderByDescending(e => e.Name) : q.OrderBy(e => e.Name),
            ["createdat"] = (q, desc) => desc ? q.OrderByDescending(e => e.CreatedAt) : q.OrderBy(e => e.CreatedAt),
            ["updatedat"] = (q, desc) => desc ? q.OrderByDescending(e => e.UpdatedAt) : q.OrderBy(e => e.UpdatedAt),
            ["starttime"] = (q, desc) => desc ? q.OrderByDescending(e => e.StartTime) : q.OrderBy(e => e.StartTime),
            ["endtime"] = (q, desc) => desc ? q.OrderByDescending(e => e.EndTime) : q.OrderBy(e => e.EndTime),
            ["totalmark"] = (q, desc) => desc ? q.OrderByDescending(e => e.TotalMark) : q.OrderBy(e => e.TotalMark),
            ["duration"] = (q, desc) => desc ? q.OrderByDescending(e => e.DurationInMinutes) : q.OrderBy(e => e.DurationInMinutes),
            ["examstatus"] = (q, desc) => desc ? q.OrderByDescending(e => e.Status) : q.OrderBy(e => e.Status),
        };
    }
}
