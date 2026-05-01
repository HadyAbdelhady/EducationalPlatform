using Application.Interfaces.BaseFilters;
using Domain.Entities;
using Domain.enums;

namespace Infrastructure.Persistence.HelperFunctions
{
    public class ExamFilterRegistry : IBaseFilterRegistry<Exam>
    {
        public Dictionary<string, Func<IQueryable<Exam>, string, IQueryable<Exam>>> Filters { get; }
            = new()
            {
                ["educationyearid"] = (q, value) =>
                    q.Where(e => e.Course != null && e.Course.EducationYearId == Guid.Parse(value)),

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
                        r.Status == Enum.Parse<ExamResultStatus>(value, true))),

                ["examtype"] = (q, value) => q.Where(e => e.ExamType.ToString().Equals(value, StringComparison.OrdinalIgnoreCase)),

                ["israndomized"] = (q, value) => q.Where(e => e.IsRandomized == bool.Parse(value)),

                ["starttime"] = (q, value) => q.Where(e => e.StartTime >= DateTimeOffset.Parse(value)),

                ["endtime"] = (q, value) => q.Where(e => e.EndTime <= DateTimeOffset.Parse(value)),


                ["name"] = (q, value) => q.Where(e => e.Name.Contains(value, StringComparison.OrdinalIgnoreCase)),

            };

        public Dictionary<string, Func<IQueryable<Exam>, bool, IOrderedQueryable<Exam>>> Sorts { get; }
            = new()
            {
                ["name"] = (q, desc) =>
                    desc ? q.OrderByDescending(e => e.Name) : q.OrderBy(e => e.Name),

                ["starttime"] = (q, desc) =>
                    desc ? q.OrderByDescending(e => e.StartTime) : q.OrderBy(e => e.StartTime),

                ["endtime"] = (q, desc) => desc ? q.OrderByDescending(e => e.EndTime) : q.OrderBy(e => e.EndTime),


                ["createdat"] = (q, desc) =>
                    desc ? q.OrderByDescending(e => e.CreatedAt) : q.OrderBy(e => e.CreatedAt),

                ["duration"] = (q, desc) => desc ? q.OrderByDescending(e => e.DurationInMinutes) : q.OrderBy(e => e.DurationInMinutes),

                ["examstatus"] = (q, desc) => desc ? q.OrderByDescending(e => e.Status) : q.OrderBy(e => e.Status),

                ["mark"] = (q, desc) =>
                     desc
                         ? q.OrderByDescending(e => e.ExamResults
                             .OrderByDescending(r => r.StudentMark)
                             .Select(r => r.StudentMark)
                             .FirstOrDefault())
                         : q.OrderBy(e => e.ExamResults
                             .OrderBy(r => r.StudentMark)
                             .Select(r => r.StudentMark)
                             .FirstOrDefault())
            };
    }
}