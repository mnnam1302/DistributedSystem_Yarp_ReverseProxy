using System.Reflection;
using DistributedSystem.Contract.JsonConverters;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Query.Domain.Abstractions.Options;
using Query.Infrastructure.DependencyInjection.Options;
using Query.Infrastructure.PipelineObservers;

namespace Query.Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddServicesInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(
            configuration.GetSection(nameof(MongoDbSettings)));

        services.AddSingleton<IMongoDbSettings>(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);
    }

    public static IServiceCollection AddMasstransitRabbitMQInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var massTransitConfiguration = new MasstransitConfiguration();
        configuration.GetSection(nameof(MasstransitConfiguration)).Bind(massTransitConfiguration);

        var messageBusOptions = new MesssageBusOptions();
        configuration.GetSection(nameof(messageBusOptions)).Bind(messageBusOptions);

        services.AddMassTransit(cfg =>
        {
            cfg.AddConsumers(Assembly.GetExecutingAssembly()); // Add all consumer to masstransit instead of above command

            // ?? => Configure endpoint formatter. Not configure for producer Root Exchange
            // Chuyển đổi - set cho class con
            cfg.SetKebabCaseEndpointNameFormatter(); // ?? Ex: Convert CreateProduct to create-user

            cfg.UsingRabbitMq((context, bus) =>
            {
                bus.Host(massTransitConfiguration.Host, massTransitConfiguration.Port, massTransitConfiguration.VHost, h =>
                {
                    h.Username(massTransitConfiguration.Username);
                    h.Password(massTransitConfiguration.Password);
                });

                bus.UseMessageRetry(retry 
                    => retry.Incremental(
                        retryLimit: messageBusOptions.RetryLimit,
                        initialInterval: messageBusOptions.InitialInterval,
                        intervalIncrement: messageBusOptions.IntervalIncrement));

                // I want to serialized when send message to RabbitMQ
                // And deserialized when receive message from RabbitMQ
                bus.UseNewtonsoftJsonSerializer();

                bus.ConfigureNewtonsoftJsonSerializer(settings =>
                {
                    settings.Converters.Add(new TypeNameHandlingConverter(TypeNameHandling.Objects));
                    settings.Converters.Add(new DateOnlyJsonConverter());
                    settings.Converters.Add(new ExpirationDateOnlyJsonConverter());

                    return settings;
                });

                bus.ConfigureNewtonsoftJsonDeserializer(settings =>
                {
                    settings.Converters.Add(new TypeNameHandlingConverter(TypeNameHandling.Objects));
                    settings.Converters.Add(new DateOnlyJsonConverter());
                    settings.Converters.Add(new ExpirationDateOnlyJsonConverter());

                    return settings;
                });

                /*
                 * Tracing and logging
                 * Những class để mình quản lý
                 * Ai đang lắng nghe - lắng nghe cái gì trên hệ thống của mình
                 * Ai đang consume - consumer cái gì
                 * Đã publish thông tin gì chưa?
                 * Đã send command gì chưa?
                 */

                bus.ConnectReceiveObserver(new LoggingReceiveObserver());
                bus.ConnectConsumeObserver(new LoggingConsumeObserver());
                bus.ConnectPublishObserver(new LoggingPublishObserver());
                bus.ConnectSendObserver(new LoggingSendObserver());

                // Rename for Root Exchange and setup Consume also
                // Exchange: MassTransitRabbitMQ.Contract.IntegrationEvents:
                // DomainEvent-SmsNotification ==> Exchange: sms-notification
                /*
                 * Chuyển đổi - set cho class cha => Phải dùng cái này
                 * => Dùng cho Root Exchange - Vì khi dùng Mass Transit nếu không quen thì nó tạo Exchange quá nhiều
                 * => Không quản lý được => Tuy không ảnh hưởng chương trình, nhưng nó tạo nhiều thôi
                 */
                bus.MessageTopology.SetEntityNameFormatter(new KebabCaseEntityNameFormatter());

                // =================== Setup for Consumer ===================

                // Important: Create Exchange and Queue
                bus.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    //<ItemGroup>
    //	<PackageReference Include = "OpenTelemetry" Version="1.7.0" />
    //	<PackageReference Include = "OpenTelemetry.Exporter.Console" Version="1.7.0" />
    //	<PackageReference Include = "OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.7.0" />
    //	<PackageReference Include = "OpenTelemetry.Extensions.Hosting" Version="1.7.0" />
    //	<PackageReference Include = "OpenTelemetry.Instrumentation.AspNetCore" Version="1.7.1" />
    //	<PackageReference Include = "OpenTelemetry.Instrumentation.Http" Version="1.7.1" />
    //	<PackageReference Include = "OpenTelemetry.Instrumentation.Runtime" Version="1.7.0" />
    //</ItemGroup>
    //public static WebApplicationBuilder AddOpenTelemetryInfrastructure(this WebApplicationBuilder builder)
    //{
    //    var otlpOptions = new OtlpOptions();
    //    builder.Configuration.GetSection(nameof(OtlpOptions)).Bind(otlpOptions);

    //    var resourceBuilder = ResourceBuilder.CreateDefault()
    //           .AddService(serviceName: otlpOptions.ServiceName,
    //                        serviceVersion: otlpOptions.ServiceVersion);

    //    var logExporter = otlpOptions.UseLogExporter.ToLowerInvariant();
    //    // Logging
    //    builder.Logging.AddOpenTelemetry(logging =>
    //    {
    //        // TODO: setup exporter here
    //        logging.SetResourceBuilder(resourceBuilder);
    //        switch (logExporter)
    //        {
    //            case "console":
    //                logging.AddConsoleExporter();
    //                break;

    //            case "otlp":
    //                logging.SetResourceBuilder(ResourceBuilder.CreateDefault()
    //                    .AddService(serviceName: otlpOptions.ServiceName,
    //                                serviceVersion: otlpOptions.ServiceVersion));

    //                logging.AddOtlpExporter(opt =>
    //                    opt.Endpoint = new Uri(otlpOptions.Endpoint));
    //                break;

    //            case "":
    //            case "none":
    //                break;
    //        }
    //    });

    //    // Metrics
    //    var metricsExporter = otlpOptions.UseMetricsExporter.ToLowerInvariant();

    //    builder.Services.AddOpenTelemetry()
    //        .WithMetrics(metrics =>
    //        {
    //            metrics.SetResourceBuilder(resourceBuilder)
    //                .AddRuntimeInstrumentation()
    //                .AddAspNetCoreInstrumentation()
    //                .AddHttpClientInstrumentation();

    //            switch (metricsExporter)
    //            {
    //                case "console":
    //                    metrics.AddConsoleExporter((exporterOptions, metricReaderOptions) =>
    //                    {
    //                        exporterOptions.Targets = ConsoleExporterOutputTargets.Console;

    //                        // The ConsoleMetricExporter defaults to a manual collect cycle.
    //                        // This configuration causes metrics to be exported to stdout on a 10s interval.
    //                        // metricReaderOptions.MetricReaderType = MetricReaderType.Periodic;
    //                        metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 10000;
    //                    });
    //                    break;

    //                case "otlp":
    //                    metrics.AddOtlpExporter(opt =>
    //                        opt.Endpoint = new Uri(otlpOptions.Endpoint));
    //                    break;

    //                case "":
    //                case "none":
    //                    break;
    //            }
    //        });

    //    // Tracing
    //    var tracingExporter = otlpOptions.UseTracingExporter.ToLowerInvariant();

    //    builder.Services.AddOpenTelemetry()
    //        .WithTracing(tracing =>
    //        {
    //            tracing
    //                .SetResourceBuilder(resourceBuilder)
    //                .AddAspNetCoreInstrumentation()
    //                .AddHttpClientInstrumentation();

    //            switch (tracingExporter)
    //            {
    //                case "console":
    //                    tracing.AddConsoleExporter();

    //                    // For options which can be bound from IConfiguration
    //                    builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(builder.Configuration.GetSection("AspNetCoreInstrumentation"));

    //                    // For options which can be configured from code only
    //                    builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(options =>
    //                        options.Filter = _ => true);

    //                    break;

    //                case "otlp":
    //                    tracing.AddOtlpExporter(otlpOtions =>
    //                        otlpOtions.Endpoint = new Uri(otlpOptions.Endpoint));
    //                    break;

    //                case "":
    //                case "none":
    //                    break;
    //            }
    //        });

    //    return builder;
    //}
}
