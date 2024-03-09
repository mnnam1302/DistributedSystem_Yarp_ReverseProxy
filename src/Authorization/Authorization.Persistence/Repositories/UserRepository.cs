using Authorization.Domain.Abstractions.Repositories;
using Authorization.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Authorization.Persistence.Repositories
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        public Task<AppUser> FindSingleUserAsync(Expression<Func<AppUser, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var users = _dbContext.Set<AppUser>().AsNoTracking();

            return users.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public void Add(AppUser entity)
        {
            _dbContext.Set<AppUser>().Add(entity);
        }
    }
}