using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Data;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class UserRepository(EducationDbContext context) : Repository<User>(context), IUserRepository
    {
        public async Task<User?> GetByGoogleEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var User = await _context.Users.Select(s => new { s.Id, s.GmailExternal })
                                            .FirstOrDefaultAsync(u => u.GmailExternal == email, cancellationToken);
            if (User == null) return null;

            return await _context.Users
                .Include(u => u.Student)
                    .ThenInclude(s => s!.EducationYear)
                .Include(u => u.Instructor)
                .FirstOrDefaultAsync(u => u.GmailExternal == email, cancellationToken);
        }

        public async Task<User?> GetStudentByIdWithRelationsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var User = await _context.Users.Select(s => new { s.Id })
                                            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (User == null) return null;

            return await _context.Users
                .Include(u => u.Student)
                .ThenInclude(s => s!.StudentExams)
                    .ThenInclude(se => se.Exam)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }
        public async Task<User?> GetInstructorByIdWithRelationsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var User = await _context.Users.Select(s => new { s.Id })
                                            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (User == null) return null;

            return await _context.Users
                .Include(u => u.Instructor)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }



    }
}
