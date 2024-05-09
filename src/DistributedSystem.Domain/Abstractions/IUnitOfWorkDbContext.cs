namespace DistributedSystem.Domain.Abstractions;

public interface IUnitOfWorkDbContext<TDbContext> : IAsyncDisposable
{
    /// <summary>
    /// Call save change from db context
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
