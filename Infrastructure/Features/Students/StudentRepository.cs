using Application.Features.Students.Interfaces;
using Domain.Entities;
using Infrastructure.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Students
{
    public class StudentRepository(EducationDbContext context) : IStudentRepository
    {
        private readonly EducationDbContext _context = context;

        public async Task<Student?> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == studentId, cancellationToken);
        }

        public async Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
