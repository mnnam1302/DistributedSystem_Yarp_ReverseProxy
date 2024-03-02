using DistributedSystem.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DistributedSystem.Persistence
{
    public class EFUnitOfWorkDbContext<TContext> : IUnitOfWorkDbContext<TContext>
        where TContext : DbContext
    {
        private readonly TContext _dbContext;

        public EFUnitOfWorkDbContext(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
            => await _dbContext.DisposeAsync();

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}