namespace Application.Interfaces
{
    /// <summary>
    /// Unit of Work pattern interface for managing transactions across multiple repositories.
    /// Ensures all repository operations within a business transaction either succeed or fail together.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the User repository.
        /// </summary>
        IUserRepository Users { get; }

        /// <summary>
        /// Gets the Course repository.
        /// </summary>
        ICourseRepository Courses { get; }

        /// <summary>
        /// Gets the RefreshToken repository.
        /// </summary>
        IRefreshTokenRepository RefreshTokens { get; }

        /// <summary>
        /// Saves all changes made in this unit of work to the database.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
