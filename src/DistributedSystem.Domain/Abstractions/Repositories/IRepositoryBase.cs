using System.Linq.Expressions;

namespace DistributedSystem.Domain.Abstractions.Repositories
{
    public interface IRepositoryBase<TEntity, in TKey>
        where TEntity : class
        // => Tran dong: In implementation should be Entity<TKey> => Why?
        // Tại sao mình không define nó ở đây luôn? TEntity : Entity<TKey> mà chỉ để TEntity : class
        // => Nếu define nó ở đây thì những class khác mà cần. Nếu chúng ta có thêm một Entity hay DomainEntity
        //     public abstract class DomainEntity<T> : IEntity<T>
        //      {
        //          public T Id { get; protected set; }
        //      }
        // => Khi implement thì th ở dưới không thể implement nữa => Không flexible
    {
        Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);

        Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);

        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Remove(TEntity entity);

        void RemoveMultiple(List<TEntity> entities);
    }
}