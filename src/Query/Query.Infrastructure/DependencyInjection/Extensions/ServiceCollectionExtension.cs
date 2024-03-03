using DistributedSystem.Contract.JsonConverters;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Query.Domain.Abstractions.Options;
using Query.Infrastructure.DependencyInjection.Options;
using Query.Infrastructure.PipelineObservers;
using System.Reflection;

namespace Query.Infrastructure.DependencyInjection.Extensions
{
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

                    bus.UseMessageRetry(retry =>
                    {
                        retry.Incremental(
                            retryLimit: messageBusOptions.RetryLimit,
                            initialInterval: messageBusOptions.InitialInterval,
                            intervalIncrement: messageBusOptions.IntervalIncrement);
                    });

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
    }
}