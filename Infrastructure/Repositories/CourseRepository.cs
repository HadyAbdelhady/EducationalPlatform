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
            var instructorCourses = _context.InstructorCourses.Where(c => c.InstructorId == instructorId)
                                                                          .Select(ic => ic.Course)
                                                                          .Include(c => c.CourseReviews)
                                                                          .Include(c => c.Sections)
                                                                          .Include(c => c.StudentCourses)
                                                                          .ToList();
            return await Task.FromResult(instructorCourses);

        }
    }
}
