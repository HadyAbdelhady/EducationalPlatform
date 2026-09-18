using Domain.Entities;

namespace Application.Features.Exams.Common
{
    public static class ExamQueryExtensions
    {
        public static IQueryable<Exam> WhereStudentEnrolled(this IQueryable<Exam> query, Guid studentId)
        {
            return query.Where(e =>
                e.Course!.StudentCourses.Any(sc => sc.StudentId == studentId) ||
                (e.SectionId != null && e.Section!.StudentSections.Any(ss => ss.StudentId == studentId)));
        }
    }
}
