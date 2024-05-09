using DistributedSystem.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace DistributedSystem.Persistence.Interceptors;

// Kế thừa SaveChangesInterceptor, trước khi SaveChange nó làm cái gì?
// DI inject nó vào thì nó chạy thôi
public sealed class ConvertDomainEventsToOutboxMessagesInterceptor
    : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext? dbContext = eventData.Context;

        if (dbContext is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var outboxMessages = dbContext.ChangeTracker
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
        dbContext.Set<OutboxMessage>().AddRange(outboxMessages);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
