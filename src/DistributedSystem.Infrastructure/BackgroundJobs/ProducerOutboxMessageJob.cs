using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Services.V1.Product;
using DistributedSystem.Persistence;
using DistributedSystem.Persistence.Outbox;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;

namespace DistributedSystem.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class ProducerOutboxMessageJob : IJob
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint; // Maybe can use Rebus library

    public ProducerOutboxMessageJob(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        // Fetching data in the OutboxMesssage table and send into RabbitMQ
        List<OutboxMessage> messages = await _dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        foreach (OutboxMessage outboxMessage in messages)
        {
            // Phải Deserialize trước về IDomainEvent để check xem nó là event gì (Là những event kế thừa IDomainEvent)
            IDomainEvent? domainEvent = JsonConvert
                .DeserializeObject<IDomainEvent>(
                    outboxMessage.Content,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All // Mình Serialize sao thì mình Deserialize như vậy
                    }
                );

            if (domainEvent is null)
                continue;

            try
            {
                switch (domainEvent.GetType().Name)
                {
                    case nameof(DomainEvent.ProductCreated):
                        var productCreated = JsonConvert
                            .DeserializeObject<DomainEvent.ProductCreated>(
                                outboxMessage.Content,
                                new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.All
                                }
                            );
                        await _publishEndpoint.Publish(productCreated, context.CancellationToken);
                        break;

                    case nameof(DomainEvent.ProductUpdated):
                        var productUpdated = JsonConvert
                            .DeserializeObject<DomainEvent.ProductUpdated>(
                                outboxMessage.Content,
                                new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.All
                                }
                            );
                        await _publishEndpoint.Publish(productUpdated, context.CancellationToken);
                        break;

                    case nameof(DomainEvent.ProductDeleted):
                        var productDeleted = JsonConvert
                            .DeserializeObject<DomainEvent.ProductDeleted>(
                                outboxMessage.Content,
                                new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.All
                                }
                            );
                        await _publishEndpoint.Publish(productDeleted, context.CancellationToken);
                        break;

                    default:
                        break;
                }

                outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                outboxMessage.Error = ex.Message;
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}
