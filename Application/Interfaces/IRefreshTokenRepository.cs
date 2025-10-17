using Domain.Entities;

namespace Application.Interfaces
{
    /// <summary>
    /// Repository interface for RefreshToken entity operations.
    /// Extends the generic repository with RefreshToken-specific methods.
    /// </summary>
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        /// <summary>
        /// Adds a new refresh token for a user.
        /// </summary>
        /// <param name="refreshToken">The refresh token string.</param>
        /// <param name="UserId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task AddRefreshTokenAsync(string refreshToken, Guid UserId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a refresh token by its token string.
        /// </summary>
        /// <param name="token">The refresh token string.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RefreshToken entity if found, null otherwise.</returns>
        Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a refresh token from the database.
        /// </summary>
        /// <param name="refreshToken">The refresh token to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task DeleteRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    }
}
