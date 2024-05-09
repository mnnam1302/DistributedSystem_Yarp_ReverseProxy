using DistributedSystem.Domain.Abstractions;
using DistributedSystem.Domain.Abstractions.Entities;
using DistributedSystem.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace DistributedSystem.Persistence;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public EFUnitOfWork(ApplicationDbContext dbContext)
        => _dbContext = dbContext;

    async ValueTask IAsyncDisposable.DisposeAsync()
        => await _dbContext.DisposeAsync();

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Quan trọng
        //ConvertDomainToOutboxMessage();
        //UpdateAuditableEntities();

        // Mình không muốn viết ở đây => Mình sẽ triển khai nó ở Interceptor
        // Kế thừa SaveChangesInterceptor, trước khi SaveChange nó làm cái gì?

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private void ConvertDomainToOutboxMessage()
    {
        // Hanlder Outbox Pattern
        // Get Domain Events từ các AggregateRoots
        // Convert thành các Outbox Message
        var outboxMessages = _dbContext.ChangeTracker
            .Entries<Domain.Abstractions.Aggregates.AggregateRoot<Guid>>()
            .Select(x => x.Entity)
            .SelectMany(aggregateRoot =>
            {
                // Get List Domain Events từ AggregateRoot
                var domainEvents = aggregateRoot.GetDomainEvents();

                // Lấy xong mình Clear nó đi
                aggregateRoot.ClearDomainEvents();

                // Trả về List Domain Event để mình xử lý tiếp sau này - Xuống dưới mình new ra từng Outbox Message
                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name, // Ví dụ, cem ProductCreated hay ProductUpdated, ProductDeleted event
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
            })
            .ToList();

        // Khi _dbContext.SaveChangesAsync() thì th ngoài kia và OutboxMessage cũng thành công
        _dbContext.Set<OutboxMessage>().AddRange(outboxMessages);
    }

    private void UpdateAuditableEntities()
    {
        IEnumerable<EntityEntry<IAuditableEntity>> entites =
            _dbContext.ChangeTracker
            .Entries<IAuditableEntity>();

        // Sau này, chúng ta tổng quát hóa lên một chút
        // Kiểu thay đổi như này, tự động lấy theo kiểu User nào tác động
        // Lấy theo kiểu Id của User login trong HttpContext
        // Có 4 trường: CreatedBy, CreatedOnUtc, ModifiedBy, ModifiedOnUtc - mình mới triển khai 2 trường

        foreach (EntityEntry<IAuditableEntity> entityEntry in entites)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(x => x.CreatedOnUtc).CurrentValue = DateTime.UtcNow;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(x => x.ModifiedOnUtc).CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
