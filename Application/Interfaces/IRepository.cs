using Domain.Interfaces;
using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);

        IQueryable<TEntity> GetAll(CancellationToken cancellationToken = default);

        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        void Update(TEntity entity);

        void UpdateRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);

        Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);

        void RemoveRange(IEnumerable<TEntity> entities);

        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
