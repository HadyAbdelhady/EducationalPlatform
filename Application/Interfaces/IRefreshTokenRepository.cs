using Domain.Entities;

namespace Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddRefreshTokenAsync(string refreshToken, Guid UserId, CancellationToken cancellationToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken);
        Task DeleteRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
