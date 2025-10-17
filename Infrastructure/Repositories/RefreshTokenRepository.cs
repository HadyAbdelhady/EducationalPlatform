using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository(EducationDbContext context) : Repository<RefreshToken>(context), IRefreshTokenRepository
    {
        public async Task AddRefreshTokenAsync(string refreshToken, Guid UserId, CancellationToken cancellationToken)
        {
            var tokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = UserId,
            };

            await AddAsync(tokenEntity, cancellationToken);
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task DeleteRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            Remove(refreshToken);
            await Task.CompletedTask;
        }
    }
}
