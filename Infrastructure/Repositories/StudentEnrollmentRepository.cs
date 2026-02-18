using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
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

        public async Task AddStudentCourseAsync(StudentCourse studentCourse, CancellationToken cancellationToken = default)
        {
            await _context.StudentCourses.AddAsync(studentCourse, cancellationToken);
        }

        public async Task AddStudentSectionAsync(StudentSection studentSection, CancellationToken cancellationToken = default)
        {
            await _context.StudentSections.AddAsync(studentSection, cancellationToken);
        }
    }
}

