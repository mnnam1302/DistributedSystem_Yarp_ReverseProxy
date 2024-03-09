using Authorization.Domain.Entities;
using System.Linq.Expressions;

namespace Authorization.Domain.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<AppUser> FindSingleUserAsync(Expression<Func<AppUser, bool>> predicate, CancellationToken cancellationToken = default);

        void Add(AppUser entity);
    }
}