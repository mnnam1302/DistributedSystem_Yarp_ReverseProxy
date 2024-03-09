using Authorization.Domain.Abstractions.Entities;
using System.Linq.Expressions;

namespace Authorization.Domain.Abstractions.Repositories
{
    public interface IRepositoryBase<IEntity, in TKey>
        where IEntity : class
    {
        Task<IEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default, 
            params Expression<Func<IEntity, object>>[] includeProperties);

        Task<IEntity> FindSingleAsync(Expression<Func<IEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, 
            params Expression<Func<IEntity, object>>[] includeProperties);

        IQueryable<IEntity> FindAll(Expression<Func<IEntity, bool>>? predicate = null, 
            params Expression<Func<IEntity, object>>[] includeProperties);

        void Add(IEntity entity);

        void Update(IEntity entity);

        void Remove(IEntity entity);

        void RemoveMultiple(List<IEntity> entities);
    }
}