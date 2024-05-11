namespace Authorization.Infrastructure.DependecyInjection.Options;

public record OtlpOptions
{
    public string ServiceName { get; init; }

    public string ServiceVersion { get; init; }

    public string Endpoint { get; init; }
}
