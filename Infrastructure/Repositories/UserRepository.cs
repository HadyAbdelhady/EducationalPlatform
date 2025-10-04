using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository(EducationDbContext context) : IUserRepository
    {
        private readonly EducationDbContext _context = context;

        public async Task<User?> GetByGoogleEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Instructor)
                .FirstOrDefaultAsync(u => u.GmailExternal == email && !u.IsDeleted, cancellationToken);
        }

        public async Task<User?> GetByIdWithRelationsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Instructor)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        }

        public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            // Explicitly add the user entity
            await _context.Users.AddAsync(user, cancellationToken);
            
            // EF Core will automatically track related entities (Student/Instructor) 
            // through navigation properties, but we ensure change detection is enabled
            _context.ChangeTracker.DetectChanges();
            
            return user;
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Update(user);
            await Task.CompletedTask;
        }
        
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
