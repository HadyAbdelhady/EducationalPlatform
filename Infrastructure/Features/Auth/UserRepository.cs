using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Infrastructure.Common.Data;
using Domain.Entities;

namespace Infrastructure.Features.Auth
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
                .Include(u => u.CenterAdmin)
                .FirstOrDefaultAsync(u => u.GmailExternal == email, cancellationToken);
        }

        public async Task<User?> GetStudentByIdWithRelationsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var User = await _context.Users.Select(s => new { s.Id })
                                            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (User == null) return null;

            return await _context.Users
                .Include(u => u.Student)
                    .ThenInclude(s => s!.ExamResults)
                        .ThenInclude(se => se.Exam)
                .Include(u => u.Student)
                    .ThenInclude(s => s!.StudentCourses)
                .Include(u => u.Student)
                    .ThenInclude(s => s!.StudentSections)
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

        public async Task<bool> DoesInstructorExistAsync(Guid instructorId, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AnyAsync(u => u.Id == instructorId && u.Instructor != null, cancellationToken);
        }

        public async Task<bool> DoesStudentExistAsync(Guid studentId, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AnyAsync(u => u.Id == studentId && u.Student != null, cancellationToken);
        }



    }
}
