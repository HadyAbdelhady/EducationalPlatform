using Application.Interfaces.BaseFilters;
using Domain.Entities;
using Domain.enums;

public class ExamFilterRegistry : IBaseFilterRegistry<Exam>
{
    public Dictionary<string, Func<IQueryable<Exam>, string, IQueryable<Exam>>> Filters { get; }
        = new()
        {
            ["courseid"] = (q, value) =>
                q.Where(e => e.CourseId == Guid.Parse(value)),

            ["instructorid"] = (q, value) =>
                q.Where(e => e.InstructorId == Guid.Parse(value)),

            ["sectionid"] = (q, value) =>
                q.Where(e => e.SectionId == Guid.Parse(value)),

            ["status"] = (q, value) =>
                q.Where(e => e.Status == Enum.Parse<ExamStatus>(value, true)),

            ["studentstatus"] = (q, value) =>
                q.Where(e => e.ExamResults.Any(r =>
                    !r.IsDeleted &&
                    r.Status == Enum.Parse<ExamResultStatus>(value, true))),

            ["name"] = (q, value) =>
                q.Where(e => e.Name.Contains(value))
        };

    public Dictionary<string, Func<IQueryable<Exam>, bool, IOrderedQueryable<Exam>>> Sorts { get; }
        = new()
        {
            ["name"] = (q, desc) =>
                desc ? q.OrderByDescending(e => e.Name) : q.OrderBy(e => e.Name),

            ["starttime"] = (q, desc) =>
                desc ? q.OrderByDescending(e => e.StartTime) : q.OrderBy(e => e.StartTime),

            ["createdat"] = (q, desc) =>
                desc ? q.OrderByDescending(e => e.CreatedAt) : q.OrderBy(e => e.CreatedAt)
        };
}
