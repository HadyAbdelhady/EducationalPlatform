using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CourseRepository(EducationDbContext context) : Repository<Course>(context), ICourseRepository
    {
        public async Task<IEnumerable<Course>> GetAllCoursesByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Where(c => c.InstructorCourses.Any(ic => ic.InstructorId == instructorId))
                .Include(c => c.CourseReviews)
                .Include(c => c.Sections)
                .Include(c => c.StudentCourses)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<Course>> GetAllCoursesByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Where(c => c.StudentCourses.Any(sc => sc.StudentId == studentId))
                .Include(c => c.CourseReviews)
                .Include(c => c.Sections)
                .Include(c => c.StudentCourses)
                .ToListAsync(cancellationToken);
        }

        public async Task<Course?> GetCourseDetailByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Include(c => c.Sections)
                .Include(c => c.InstructorCourses)
                    .ThenInclude(ic => ic.Instructor)
                    .ThenInclude(i => i.User)
                .Include(c => c.StudentCourses)
                .Include(c => c.CourseReviews)
                    .ThenInclude(r => r.Student)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);
        }
    }
}
