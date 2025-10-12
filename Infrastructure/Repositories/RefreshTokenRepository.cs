using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository(EducationDbContext educationDbContext) : IRefreshTokenRepository
    {
        public EducationDbContext EducationDbContext { get; } = educationDbContext;

        public async Task AddRefreshTokenAsync(string refreshToken,Guid UserId, CancellationToken cancellationToken)
        {
            var tokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = UserId,
            };

            await EducationDbContext.RefreshTokens.AddAsync(tokenEntity, cancellationToken);
            await EducationDbContext.SaveChangesAsync(cancellationToken);

        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await EducationDbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task DeleteRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            EducationDbContext.RefreshTokens.Remove(refreshToken);
            await EducationDbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await EducationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
