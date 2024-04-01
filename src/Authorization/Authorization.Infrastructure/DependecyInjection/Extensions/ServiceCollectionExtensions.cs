using Authorization.Application.Abstractions;
using Authorization.Infrastructure.Authentication;
using Authorization.Infrastructure.Caching;
using Authorization.Infrastructure.DependecyInjection.Options;
using Authorization.Infrastructure.PasswordHasher;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Authorization.Infrastructure.DependecyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServicesInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IJwtTokenService, JwtTokenService>();
        services.AddTransient<ICacheService, CacheService>();
        services.AddTransient<IPasswordHasherService, PasswordHasherService>();
    }

    public static void AddRedisInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            var connectionString = configuration.GetConnectionString("Redis");
            redisOptions.Configuration = connectionString;
        });
    }

    public static WebApplicationBuilder AddOpenTelemetryInfrastructure(this WebApplicationBuilder builder)
    {
        var otlpOptions = new OtlpOptions();
        builder.Configuration.GetSection(nameof(OtlpOptions)).Bind(otlpOptions);

        var resourceBuilder = ResourceBuilder.CreateDefault()
               .AddService(serviceName: otlpOptions.ServiceName,
                            serviceVersion: otlpOptions.ServiceVersion);

        var logExporter = otlpOptions.UseLogExporter.ToLowerInvariant();
        // Logging
        builder.Logging.AddOpenTelemetry(logging =>
        {
            // TODO: setup exporter here
            logging.SetResourceBuilder(resourceBuilder);
            switch (logExporter)
            {
                case "console":
                    logging.AddConsoleExporter();
                    break;
                case "otlp":
                    logging.SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(serviceName: otlpOptions.ServiceName,
                                    serviceVersion: otlpOptions.ServiceVersion));

                    logging.AddOtlpExporter(opt =>
                        opt.Endpoint = new Uri(otlpOptions.Endpoint));
                    break;
                case "":
                case "none":
                    break;
            }
        });

        // Metrics
        var metricsExporter = otlpOptions.UseMetricsExporter.ToLowerInvariant();

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.SetResourceBuilder(resourceBuilder)
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                switch (metricsExporter)
                {
                    case "console":
                        metrics.AddConsoleExporter((exporterOptions, metricReaderOptions) =>
                        {
                            exporterOptions.Targets = ConsoleExporterOutputTargets.Console;

                            // The ConsoleMetricExporter defaults to a manual collect cycle.
                            // This configuration causes metrics to be exported to stdout on a 10s interval.
                            // metricReaderOptions.MetricReaderType = MetricReaderType.Periodic;
                            metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 10000;
                        });
                        break;
                    case "otlp":
                        metrics.AddOtlpExporter(opt =>
                            opt.Endpoint = new Uri(otlpOptions.Endpoint));
                        break;
                    case "":
                    case "none":
                        break;
                }
            });

        // Tracing
        var tracingExporter = otlpOptions.UseTracingExporter.ToLowerInvariant();

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                switch (tracingExporter)
                {
                    case "console":
                        tracing.AddConsoleExporter();

                        // For options which can be bound from IConfiguration
                        builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(builder.Configuration.GetSection("AspNetCoreInstrumentation"));

                        // For options which can be configured from code only
                        builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(options =>
                            options.Filter = _ => true);

                        break;
                    case "otlp":
                        tracing.AddOtlpExporter(otlpOtions =>
                            otlpOtions.Endpoint = new Uri(otlpOptions.Endpoint));
                        break;
                    case "":
                    case "none":
                        break;
                }
            });

        return builder;
    }
}