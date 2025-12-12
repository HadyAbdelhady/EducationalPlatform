using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
                    .ThenInclude(s => s!.EducationYearId)
                .Include(u => u.Instructor)
                .FirstOrDefaultAsync(u => u.GmailExternal == email, cancellationToken);
        }

        public async Task<User?> GetByIdWithRelationsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Instructor)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }
    }
}
