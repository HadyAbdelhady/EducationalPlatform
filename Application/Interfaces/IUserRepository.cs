using Domain.Entities;

namespace Application.Interfaces
{
    /// <summary>
    /// Repository interface for User entity operations.
    /// Extends the generic repository with User-specific methods.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Finds a user by their Google email address.
        /// </summary>
        /// <param name="email">Google email address.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>User entity if found, null otherwise.</returns>
        Task<User?> GetByGoogleEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a user by their ID including related Student or Instructor entities.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>User entity if found, null otherwise.</returns>
        Task<User?> GetByIdWithRelationsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
