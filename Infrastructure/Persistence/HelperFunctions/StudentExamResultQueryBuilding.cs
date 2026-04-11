using Application.Interfaces.BaseFilters;
using Domain.Entities;
using Domain.enums;

namespace Infrastructure.Persistence.HelperFunctions
{
    public class StudentExamResultFilterRegistry : IBaseFilterRegistry<StudentExamResult>
    {
        public Dictionary<string, Func<IQueryable<StudentExamResult>, string, IQueryable<StudentExamResult>>> Filters { get; }
            = new()
            {
                ["status"] = (q, value) =>
                    q.Where(er => er.Status == Enum.Parse<ExamResultStatus>(value, true)),

                ["studentid"] = (q, value) =>
                    q.Where(er => er.StudentId == Guid.Parse(value)),

                ["examid"] = (q, value) =>
                    q.Where(er => er.ExamId == Guid.Parse(value)),

                ["studentname"] = (q, value) =>
                    q.Where(er => er.Student.User.FullName.Contains(value)),

                ["ispassed"] = (q, value) =>
                    q.Where(er => er.StudentMark.HasValue &&
                        er.StudentMark.Value >= er.Exam.TotalMark * 0.6m == bool.Parse(value)),

                ["iscompleted"] = (q, value) =>
                    q.Where(er => (er.Status == ExamResultStatus.Passed || er.Status == ExamResultStatus.Failed) == bool.Parse(value))
            };

        public Dictionary<string, Func<IQueryable<StudentExamResult>, bool, IOrderedQueryable<StudentExamResult>>> Sorts { get; }
            = new()
            {
                ["studentname"] = (q, desc) =>
                    desc ? q.OrderByDescending(er => er.Student.User.FullName) : q.OrderBy(er => er.Student.User.FullName),

                ["status"] = (q, desc) =>
                    desc ? q.OrderByDescending(er => er.Status) : q.OrderBy(er => er.Status),

                ["obtainedmarks"] = (q, desc) =>
                    desc ? q.OrderByDescending(er => er.StudentMark) : q.OrderBy(er => er.StudentMark),

                ["takenat"] = (q, desc) =>
                    desc ? q.OrderByDescending(er => er.TakenAt) : q.OrderBy(er => er.TakenAt),

                ["createdat"] = (q, desc) =>
                    desc ? q.OrderByDescending(er => er.CreatedAt) : q.OrderBy(er => er.CreatedAt),

                ["updatedat"] = (q, desc) =>
                    desc ? q.OrderByDescending(er => er.UpdatedAt) : q.OrderBy(er => er.UpdatedAt)
            };
    }
}