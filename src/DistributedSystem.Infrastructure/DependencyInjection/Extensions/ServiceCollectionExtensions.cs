using DistributedSystem.Application.Abstractions;
using DistributedSystem.Contract.JsonConverters;
using DistributedSystem.Infrastructure.Authentication;
using DistributedSystem.Infrastructure.BackgroundJobs;
using DistributedSystem.Infrastructure.Caching;
using DistributedSystem.Infrastructure.DependencyInjection.Options;
using DistributedSystem.Infrastructure.PipelineObservers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quartz;
using System.Reflection;

namespace DistributedSystem.Infrastructure.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServicesInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IJwtTokenService, JwtTokenService>();
            services.AddTransient<ICacheService, CacheService>();
        }

        public static void AddRedisInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Nếu mình không cấu hình AddStackExchangeRedisCache thì sẽ sử dụng MemoryCache
            services.AddStackExchangeRedisCache(redisOptions =>
            {
                var connectionString = configuration.GetConnectionString("Redis");
                redisOptions.Configuration = connectionString;
            });
        }

        // Configure MassTransit with RabbitMQ
        public static IServiceCollection AddMasstransitRabbitMQInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var massTransitConfiguration = new MasstransitConfiguration();
            configuration.GetSection(nameof(MasstransitConfiguration)).Bind(massTransitConfiguration);

            var messageBusOptions = new MesssageBusOptions();
            configuration.GetSection(nameof(MesssageBusOptions)).Bind(messageBusOptions);

            services.AddMassTransit(cfg =>
            {
                // =================== Setup for Consumer ===================
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

        // Configure Job
        public static void AddQuartzInfrastructure(this IServiceCollection services)
        {
            services.AddQuartz(configure =>
            {
                var jobKey = new JobKey(nameof(ProducerOutboxMessageJob));

                // Add job and trigger for this job
                // Mục đích: mỗi lần mình sẽ Push 20 message lên RabbitMQ
                configure
                    .AddJob<ProducerOutboxMessageJob>(jobKey)
                    .AddTrigger(trigger =>
                    {
                        trigger.ForJob(jobKey)
                        .WithSimpleSchedule(schedule =>
                        {
                            // Check lại Milisecond hay Microsecond - Trandong
                            schedule.WithInterval(TimeSpan.FromMilliseconds(100));
                            schedule.RepeatForever();
                        });
                    });

                configure.UseMicrosoftDependencyInjectionJobFactory();
            });

            services.AddQuartzHostedService();
        }

        public static void AddMediatRInfrastructure(this IServiceCollection services)
        {
            // Tại sao ở đây lại có thêm Validator => MesssageBusOptions có các ràng buộc
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly));
        }
    }
}