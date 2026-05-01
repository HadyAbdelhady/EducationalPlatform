using Application.DTOs.Exam;
using Application.Interfaces.BaseFilters;

namespace Infrastructure.Persistence.HelperFunctions
{
    public class InstructorExamsFilterRegistry : IBaseFilterRegistry<InstructorExamsResponseDto>
    {
        public Dictionary<string, Func<IQueryable<InstructorExamsResponseDto>, string, IQueryable<InstructorExamsResponseDto>>> Filters { get; } = new()
        {
            ["courseid"] = (q, value) => q.Where(e => e.CourseId == Guid.Parse(value)),
            ["sectionid"] = (q, value) => q.Where(e => e.SectionId == Guid.Parse(value)),
            ["examstatus"] = (q, value) => q.Where(e => e.ExamStatus.ToString().Equals(value, StringComparison.OrdinalIgnoreCase)),
            ["examtype"] = (q, value) => q.Where(e => e.ExamType.ToString().Equals(value, StringComparison.OrdinalIgnoreCase)),
            ["name"] = (q, value) => q.Where(e => e.Name.Contains(value, StringComparison.OrdinalIgnoreCase)),
            ["israndomized"] = (q, value) => q.Where(e => e.IsRandomized == bool.Parse(value)),
            ["starttime"] = (q, value) => q.Where(e => e.StartTime >= DateTimeOffset.Parse(value)),
            ["endtime"] = (q, value) => q.Where(e => e.EndTime <= DateTimeOffset.Parse(value)),
        };

        public Dictionary<string, Func<IQueryable<InstructorExamsResponseDto>, bool, IOrderedQueryable<InstructorExamsResponseDto>>> Sorts { get; } = new()
        {
            ["name"] = (q, desc) => desc ? q.OrderByDescending(e => e.Name) : q.OrderBy(e => e.Name),
            ["createdat"] = (q, desc) => desc ? q.OrderByDescending(e => e.CreatedAt) : q.OrderBy(e => e.CreatedAt),
            ["updatedat"] = (q, desc) => desc ? q.OrderByDescending(e => e.UpdatedAt) : q.OrderBy(e => e.UpdatedAt),
            ["starttime"] = (q, desc) => desc ? q.OrderByDescending(e => e.StartTime) : q.OrderBy(e => e.StartTime),
            ["endtime"] = (q, desc) => desc ? q.OrderByDescending(e => e.EndTime) : q.OrderBy(e => e.EndTime),
            ["totalmark"] = (q, desc) => desc ? q.OrderByDescending(e => e.TotalMark) : q.OrderBy(e => e.TotalMark),
            ["duration"] = (q, desc) => desc ? q.OrderByDescending(e => e.DurationInMinutes) : q.OrderBy(e => e.DurationInMinutes),
            ["examstatus"] = (q, desc) => desc ? q.OrderByDescending(e => e.ExamStatus) : q.OrderBy(e => e.ExamStatus),
            ["coursename"] = (q, desc) => desc ? q.OrderByDescending(e => e.CourseName) : q.OrderBy(e => e.CourseName),
            ["sectionname"] = (q, desc) => desc ? q.OrderByDescending(e => e.SectionName) : q.OrderBy(e => e.SectionName),
        };
    }
}
