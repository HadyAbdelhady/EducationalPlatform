using Application.Interfaces.BaseFilters;
using Domain.Entities;

namespace Infrastructure.Persistence.HelperFunctions
{
    public class AnswersSheetFilterRegistry : IBaseFilterRegistry<AnswersSheet>
    {
        public Dictionary<string, Func<IQueryable<AnswersSheet>, string, IQueryable<AnswersSheet>>> Filters { get; } = new()
        {
            ["studentid"] = (q, value) => q.Where(a => a.StudentId == Guid.Parse(value)),
            ["questionsheetid"] = (q, value) => q.Where(a => a.QuestionsSheetId == Guid.Parse(value)),
            ["isapproved"] = (q, value) => q.Where(a => a.IsApproved == bool.Parse(value)),
            ["instructorid"] = (q, value) => q.Where(a => a.QuestionsSheet.InstructorId == Guid.Parse(value)),
            ["name"] = (q, value) => q.Where(a => a.Name.Contains(value)),
        };

        public Dictionary<string, Func<IQueryable<AnswersSheet>, bool, IOrderedQueryable<AnswersSheet>>> Sorts { get; } = new()
        {
            ["name"] = (q, desc) => desc ? q.OrderByDescending(a => a.Name) : q.OrderBy(a => a.Name),
            ["createdat"] = (q, desc) => desc ? q.OrderByDescending(a => a.CreatedAt) : q.OrderBy(a => a.CreatedAt),
            ["updatedat"] = (q, desc) => desc ? q.OrderByDescending(a => a.UpdatedAt) : q.OrderBy(a => a.UpdatedAt),
        };
    }
}
