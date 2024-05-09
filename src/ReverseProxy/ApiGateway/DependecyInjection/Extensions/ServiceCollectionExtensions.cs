using ApiGateway.Abstractions;
using ApiGateway.Caching;
using ApiGateway.DependecyInjection.Options;

namespace ApiGateway.DependecyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddYarpReverseProxyApiGateway(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReverseProxy()
             .LoadFromConfig(configuration.GetSection("ReverseProxy"));
    }

    public static void AddServicesApiGateway(this IServiceCollection services)
    {
        services.AddTransient<ICacheService, CacheService>();
    }

    public static void AddRedisApiGateway(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            var connectionString = configuration.GetConnectionString("Redis");
            options.Configuration = connectionString;
        });
    }

 //   <ItemGroup>
	//	<PackageReference Include = "OpenTelemetry" Version="1.7.0" />
	//	<PackageReference Include = "OpenTelemetry.Exporter.Console" Version="1.7.0" />
	//	<PackageReference Include = "OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.7.0" />
	//	<PackageReference Include = "OpenTelemetry.Extensions.Hosting" Version="1.7.0" />
	//	<PackageReference Include = "OpenTelemetry.Instrumentation.AspNetCore" Version="1.7.1" />
	//	<PackageReference Include = "OpenTelemetry.Instrumentation.Http" Version="1.7.1" />
	//	<PackageReference Include = "OpenTelemetry.Instrumentation.Runtime" Version="1.7.0" />
	//</ItemGroup>
 //   public static WebApplicationBuilder AddOpenTelemetryInfrastructure(this WebApplicationBuilder builder)
 //   {
 //       var otlpOptions = new OtlpOptions();
 //       builder.Configuration.GetSection(nameof(OtlpOptions)).Bind(otlpOptions);

 //       var resourceBuilder = ResourceBuilder.CreateDefault()
 //              .AddService(serviceName: otlpOptions.ServiceName,
 //                           serviceVersion: otlpOptions.ServiceVersion);

 //       var logExporter = otlpOptions.UseLogExporter.ToLowerInvariant();
 //       // Logging
 //       builder.Logging.AddOpenTelemetry(logging =>
 //       {
 //           // TODO: setup exporter here
 //           logging.SetResourceBuilder(resourceBuilder);
 //           switch (logExporter)
 //           {
 //               case "console":
 //                   logging.AddConsoleExporter();
 //                   break;
 //               case "otlp":
 //                   logging.SetResourceBuilder(ResourceBuilder.CreateDefault()
 //                       .AddService(serviceName: otlpOptions.ServiceName,
 //                                   serviceVersion: otlpOptions.ServiceVersion));

 //                   logging.AddOtlpExporter(opt =>
 //                       opt.Endpoint = new Uri(otlpOptions.Endpoint));
 //                   break;
 //               case "":
 //               case "none":
 //                   break;
 //           }
 //       });

 //       // Metrics
 //       var metricsExporter = otlpOptions.UseMetricsExporter.ToLowerInvariant();

 //       builder.Services.AddOpenTelemetry()
 //           .WithMetrics(metrics =>
 //           {
 //               metrics.SetResourceBuilder(resourceBuilder)
 //                   .AddRuntimeInstrumentation()
 //                   .AddAspNetCoreInstrumentation()
 //                   .AddHttpClientInstrumentation();

 //               switch (metricsExporter)
 //               {
 //                   case "console":
 //                       metrics.AddConsoleExporter((exporterOptions, metricReaderOptions) =>
 //                       {
 //                           exporterOptions.Targets = ConsoleExporterOutputTargets.Console;

 //                           // The ConsoleMetricExporter defaults to a manual collect cycle.
 //                           // This configuration causes metrics to be exported to stdout on a 10s interval.
 //                           // metricReaderOptions.MetricReaderType = MetricReaderType.Periodic;
 //                           metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 10000;
 //                       });
 //                       break;
 //                   case "otlp":
 //                       metrics.AddOtlpExporter(opt =>
 //                           opt.Endpoint = new Uri(otlpOptions.Endpoint));
 //                       break;
 //                   case "":
 //                   case "none":
 //                       break;
 //               }
 //           });

 //       // Tracing
 //       var tracingExporter = otlpOptions.UseTracingExporter.ToLowerInvariant();

 //       builder.Services.AddOpenTelemetry()
 //           .WithTracing(tracing =>
 //           {
 //               tracing
 //                   .SetResourceBuilder(resourceBuilder)
 //                   .AddAspNetCoreInstrumentation()
 //                   .AddHttpClientInstrumentation();

 //               switch (tracingExporter)
 //               {
 //                   case "console":
 //                       tracing.AddConsoleExporter();

 //                       // For options which can be bound from IConfiguration
 //                       builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(builder.Configuration.GetSection("AspNetCoreInstrumentation"));

 //                       // For options which can be configured from code only
 //                       builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(options =>
 //                           options.Filter = _ => true);

 //                       break;
 //                   case "otlp":
 //                       tracing.AddOtlpExporter(otlpOtions =>
 //                           otlpOtions.Endpoint = new Uri(otlpOptions.Endpoint));
 //                       break;
 //                   case "":
 //                   case "none":
 //                       break;
 //               }
 //           });

 //       return builder;
 //   }
}
