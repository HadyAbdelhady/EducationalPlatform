using Application.Common.Interfaces;
using Application.Features.HomeScreen.Interfaces;
using Domain.Entities;
using Domain.enums;
using Infrastructure.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.HomeScreen
{
    public class StudentEnrollmentRepository(EducationDbContext context) : IStudentEnrollmentRepository
    {
        private readonly EducationDbContext _context = context;

        public async Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentCourses.AnyAsync(sc => sc.StudentId == studentId &&
                                                          sc.CourseId == courseId,
                                                          cancellationToken);
        }

        public async Task<bool> IsStudentEnrolledInSectionAsync(Guid studentId, Guid sectionId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentSections
                .AnyAsync(ss => ss.StudentId == studentId &&
                                ss.SectionId == sectionId, cancellationToken);
        }

        public async Task<bool> CanStudentAccessSectionContentAsync(Guid studentId, Guid sectionId, CancellationToken cancellationToken = default)
        {
            if (await _context.StudentSections.AnyAsync(
                    ss => ss.StudentId == studentId && ss.SectionId == sectionId,
                    cancellationToken))
            {
                return true;
            }

            return await (
                from sc in _context.StudentCourses
                join sec in _context.Sections on sc.CourseId equals sec.CourseId
                where sc.StudentId == studentId && sec.Id == sectionId
                select sc
            ).AnyAsync(cancellationToken);
        }

        public async Task AddStudentCourseAsync(StudentCourse studentCourse, CancellationToken cancellationToken = default)
        {
            await _context.StudentCourses.AddAsync(studentCourse, cancellationToken);
        }

        public async Task AddStudentSectionAsync(StudentSection studentSection, CancellationToken cancellationToken = default)
        {
            await _context.StudentSections.AddAsync(studentSection, cancellationToken);
        }

        public async Task<decimal> GetRemainingCoursePriceAsync(
            Guid studentId,
            Guid courseId,
            decimal catalogPrice,
            CancellationToken cancellationToken = default)
        {
            var paid = await _context.Payments
                .AsNoTracking()
                .Where(p => p.StudentId == studentId &&
                            p.Status == PaymentStatus.Completed &&
                            p.SectionId != null &&
                            _context.Sections.Any(s => s.Id == p.SectionId && s.CourseId == courseId))
                .SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m;

            return Math.Max(0m, catalogPrice - paid);
        }

        public async Task EnrollFromPaymentAsync(
            Guid studentId,
            Guid? courseId,
            Guid? sectionId,
            CancellationToken cancellationToken = default)
        {
            // Section purchases also store CourseId for revenue; prefer SectionId when present.
            if (sectionId.HasValue)
            {
                if (await IsStudentEnrolledInSectionAsync(studentId, sectionId.Value, cancellationToken))
                    return;

                var section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == sectionId.Value, cancellationToken);
                if (section is null)
                    return;

                var alreadyCountedOnCourse = await IsStudentEnrolledInCourseAsync(studentId, section.CourseId, cancellationToken)
                    || await _context.StudentSections.AnyAsync(
                        ss => ss.StudentId == studentId && ss.Section.CourseId == section.CourseId,
                        cancellationToken);

                await AddStudentSectionAsync(
                    new StudentSection
                    {
                        StudentId = studentId,
                        SectionId = sectionId.Value
                    },
                    cancellationToken);

                section.NumberOfStudentsEnrolled++;

                if (!alreadyCountedOnCourse)
                {
                    var courseForSection = await _context.Courses.FirstOrDefaultAsync(c => c.Id == section.CourseId, cancellationToken);
                    if (courseForSection is not null)
                        courseForSection.NumberOfStudentsEnrolled++;
                }

                return;
            }

            if (!courseId.HasValue)
                return;

            if (await IsStudentEnrolledInCourseAsync(studentId, courseId.Value, cancellationToken))
                return;

            var alreadyCountedAsStudent = await _context.StudentSections
                .AnyAsync(ss => ss.StudentId == studentId && ss.Section.CourseId == courseId.Value, cancellationToken);

            await AddStudentCourseAsync(
                new StudentCourse
                {
                    StudentId = studentId,
                    CourseId = courseId.Value
                },
                cancellationToken);

            if (!alreadyCountedAsStudent)
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId.Value, cancellationToken);
                if (course is not null)
                    course.NumberOfStudentsEnrolled++;
            }
        }
    }
}
