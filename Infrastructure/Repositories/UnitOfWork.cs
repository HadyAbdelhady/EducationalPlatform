using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Unit of Work implementation that manages transactions across multiple repositories.
    /// </summary>
    public class UnitOfWork(EducationDbContext context, IServiceProvider provider) : IUnitOfWork
    {
        private readonly EducationDbContext _context = context;
        private readonly IServiceProvider _provider = provider;
        private IDbContextTransaction? _transaction;

        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        public IRepository<T> Repository<T>() where T : class, Domain.Interfaces.IEntity
        {
            var type = typeof(T);
            // Try to get cached one
            if (_repositories.TryGetValue(type, out var repoObj))
                return (IRepository<T>)repoObj;

            // Resolve from DI (open generic registration will return Repository<T>)
            var repo = _provider.GetRequiredService<IRepository<T>>();
            _repositories[type] = repo!;
            return repo!;
        }

        public TRepo GetRepository<TRepo>() where TRepo : class
        {
            return _provider.GetRequiredService<TRepo>();
        }


        /// <summary>
        /// Saves all changes made in this unit of work to the database.
        /// </summary>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Disposes the unit of work and releases resources.
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
