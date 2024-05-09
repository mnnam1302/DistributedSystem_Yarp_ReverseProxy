using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DistributedSystem.Domain.Abstractions.Repositories;

public interface IRepositoryBaseDbContext<TContext, TEntity, TKey>
    where TContext : DbContext
    where TEntity : class // => In implementation should be Entity<TKey>
{
    Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);

    Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);

    IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties);

    void Add(TEntity entity);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    void RemoveMultiple(List<TEntity> entities);
}
