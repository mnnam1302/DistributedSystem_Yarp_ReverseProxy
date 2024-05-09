namespace Query.Infrastructure.DependencyInjection.Options;

public class OtlpOptions
{
    public string UseTracingExporter { get; set; }

    public string UseMetricsExporter { get; set; }

    public string UseLogExporter { get; set; }

    public string ServiceName { get; set; }

    public string ServiceVersion { get; set; }

    public string Endpoint { get; set; }
}
