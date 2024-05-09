using System.ComponentModel.DataAnnotations;

namespace Authorization.Persistence.DependencyInjection.Options;

public record SqlServerRetryOptions
{
    [Required, Range(5, 20)]
    public int MaxRetryCount { get; init; }

    [Required, Timestamp]
    public TimeSpan MaxRetryDelay { get; init; }

    public int[]? ErrorNumbersoAdd { get; init; }
}
