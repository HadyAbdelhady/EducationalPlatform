namespace Application.Common.Interfaces
{
    /// <summary>
    /// Resolves the authenticated user id from JWT claims.
    /// In Development only, <c>Auth:DevUserId</c> can override claims for local testing.
    /// </summary>
    public interface ICurrentUserService
    {
        bool TryGetUserId(out Guid userId);

        /// <summary>
        /// Returns the current user id, or throws <see cref="UnauthorizedAccessException"/> if missing.
        /// </summary>
        Guid GetUserId();
    }
}
