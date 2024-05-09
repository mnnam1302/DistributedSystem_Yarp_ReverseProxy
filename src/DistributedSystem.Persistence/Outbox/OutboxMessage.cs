namespace DistributedSystem.Persistence.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }

    public string Type { get; set; }

    public string Content { get; set; }

    public DateTimeOffset OccurredOnUtc { get; set; }

    public DateTimeOffset? ProcessedOnUtc { get; set; }

    public string? Error { get; set; }
}
