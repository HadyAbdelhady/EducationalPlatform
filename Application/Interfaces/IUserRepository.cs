using Domain.Entities;

namespace Application.Interfaces
{
    /// <summary>
    /// Repository interface for User entity operations.
    /// </summary>
    public interface IUserRepository
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

        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        /// <param name="user">User entity to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Created user entity.</returns>
        Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <param name="user">User entity to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completed task.</returns>
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves all changes to the database.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
