using Application.Common.Interfaces;
using Domain;
using Domain.Entities;
using Domain.enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Exams
{
    public class ExamFilterRegistry : IBaseFilterRegistry<Exam>
    {
        public Dictionary<string, Func<IQueryable<Exam>, string, IQueryable<Exam>>> Filters { get; }
            = new()
            {
                ["educationyearid"] = (q, value) =>
                    q.Where(e => e.Course != null && e.Course.EducationYearId == Guid.Parse(value)),

                ["courseid"] = (q, value) =>
                    q.Where(e => e.CourseId == Guid.Parse(value) && e.SectionId == null),

                ["instructorid"] = (q, value) =>
                    q.Where(e => e.InstructorId == Guid.Parse(value)),

                ["sectionid"] = (q, value) =>
                    q.Where(e => e.SectionId == Guid.Parse(value)),

                ["status"] = (q, value) =>
                {
                    if (!Enum.TryParse<ExamStatus>(value, true, out var status))
                        return q.Where(e => false);

                    var now = EgyptTime.UtcNow;
                    return status switch
                    {
                        ExamStatus.Draft => q.Where(e => e.Status == ExamStatus.Draft),
                        ExamStatus.Scheduled => q.Where(e => e.Status != ExamStatus.Draft && e.Status != ExamStatus.Finished && (e.StartTime == null || e.StartTime > now) && (e.EndTime == null || e.EndTime > now)),
                        ExamStatus.Started => q.Where(e => e.Status != ExamStatus.Draft && e.Status != ExamStatus.Finished && e.StartTime <= now && (e.EndTime == null || e.EndTime > now)),
                        ExamStatus.Finished => q.Where(e => e.Status == ExamStatus.Finished || (e.Status != ExamStatus.Draft && e.EndTime != null && e.EndTime <= now)),
                        _ => q
                    };
                },

                ["studentstatus"] = (q, value) =>
                    Enum.TryParse<ExamResultStatus>(value, true, out var status)
                        ? (status == ExamResultStatus.NotStarted
                            ? q.Where(e => !e.ExamResults.Any(r => r.Status == ExamResultStatus.InProgress || r.Status == ExamResultStatus.Passed || r.Status == ExamResultStatus.Failed))
                            : q.Where(e => e.ExamResults.Any(r => r.Status == status)))
                        : q,


                ["examtype"] = (q, value) =>
                    Enum.TryParse<ExamType>(value, true, out var type)
                        ? q.Where(e => e.ExamType == type)
                        : q.Where(e => false),

                ["israndomized"] = (q, value) => q.Where(e => e.IsRandomized == bool.Parse(value)),

                ["starttime"] = (q, value) => q.Where(e => e.StartTime >= DateTimeOffset.Parse(value)),

                ["endtime"] = (q, value) => q.Where(e => e.EndTime <= DateTimeOffset.Parse(value)),

                ["name"] = (q, value) => q.Where(e => e.Name.Contains(value, StringComparison.Ordinal)),

                ["studentid"] = (q, value) =>
                    q.Where(e =>
                        e.Course!.StudentCourses.Any(sc => sc.StudentId == Guid.Parse(value)) ||
                        (e.SectionId != null && e.Section!.StudentSections.Any(ss => ss.StudentId == Guid.Parse(value))))
                     .Include(e => e.ExamResults.Where(r => r.StudentId == Guid.Parse(value))),

                ["examstatus"] = (q, value) =>
                {
                    if (!Enum.TryParse<ExamStatus>(value, true, out var status))
                        return q.Where(e => false);

                    var now = EgyptTime.UtcNow;
                    return status switch
                    {
                        ExamStatus.Draft => q.Where(e => e.Status == ExamStatus.Draft),
                        ExamStatus.Scheduled => q.Where(e => e.Status != ExamStatus.Draft && e.Status != ExamStatus.Finished && (e.StartTime == null || e.StartTime > now) && (e.EndTime == null || e.EndTime > now)),
                        ExamStatus.Started => q.Where(e => e.Status != ExamStatus.Draft && e.Status != ExamStatus.Finished && e.StartTime <= now && (e.EndTime == null || e.EndTime > now)),
                        ExamStatus.Finished => q.Where(e => e.Status == ExamStatus.Finished || (e.Status != ExamStatus.Draft && e.EndTime != null && e.EndTime <= now)),
                        _ => q
                    };
                },
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