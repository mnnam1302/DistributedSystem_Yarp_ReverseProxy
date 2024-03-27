using DistributedSystem.Domain.Abstractions;
using DistributedSystem.Domain.Abstractions.Repositories;
using DistributedSystem.Persistence.DependencyInjection.Options;
using DistributedSystem.Persistence.Interceptors;
using DistributedSystem.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DistributedSystem.Persistence.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSqlPersistence(this IServiceCollection services)
        {
            services.AddDbContextPool<DbContext, ApplicationDbContext>((provider, builder) =>
            {
                // Interceptor
                // Lưu ý: Phải add SINGLETON 2 th này vào DI, thì mới Get Service ra được
                var outboxInterceptor = provider.GetService<ConvertDomainEventsToOutboxMessagesInterceptor>();
                var auditableInterceptor = provider.GetService<UpdateAuditableEntitiesInterceptor>();

                var configuration = provider.GetRequiredService<IConfiguration>();
                var options = provider.GetRequiredService<IOptionsMonitor<SqlServerRetryOptions>>();

                #region ============== SQL-SERVER-STRATEGY-1 ==============

                builder
                .EnableDetailedErrors(true)
                .EnableSensitiveDataLogging(true)
                .UseLazyLoadingProxies(true) // => If LazyLoadingProxies, all of the Navigation fields should be VIRTUAL
                .UseSqlServer(
                    connectionString: configuration.GetConnectionString("ConnectionStrings"),
                    sqlServerOptionsAction: optionsBuilder
                        => optionsBuilder
                        .ExecutionStrategy(
                            dependencies => new SqlServerRetryingExecutionStrategy(
                                dependencies: dependencies,
                                maxRetryCount: options.CurrentValue.MaxRetryCount,
                                maxRetryDelay: options.CurrentValue.MaxRetryDelay,
                                errorNumbersToAdd: options.CurrentValue.ErrorNumbersoAdd))
                        .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name))
                .AddInterceptors(outboxInterceptor,
                    auditableInterceptor);
                // MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name)
                // lấy tên của assembly chứa lớp ApplicationDbContext, từ đó chỉ định tên của assembly chứa các migration.


                #endregion ============== SQL-SERVER-STRATEGY-1 ==============

                #region ============== SQL-SERVER-STRATEGY-2 ==============

                //builder
                //.EnableDetailedErrors(true)
                //.EnableSensitiveDataLogging(true)
                //.UseLazyLoadingProxies(true) // => If UseLazyLoadingProxies, all of the navigation fields should be VIRTUAL
                //.UseSqlServer(
                //    connectionString: configuration.GetConnectionString("ConnectionStrings"),
                //        sqlServerOptionsAction: optionsBuilder
                //            => optionsBuilder
                //            .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name));

                #endregion ============== SQL-SERVER-STRATEGY-2 ==============
            });
        }

        public static void AddInterceptorPersistence(this IServiceCollection services)
        {
            services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();
            services.AddSingleton<UpdateAuditableEntitiesInterceptor>();
        }

        public static void AddRepositoryPersistence(this IServiceCollection services)
        {
            services.AddTransient(typeof(IUnitOfWork), typeof(EFUnitOfWork));
            services.AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));

            services.AddTransient(typeof(IUnitOfWorkDbContext<>), typeof(EFUnitOfWorkDbContext<>));
            services.AddTransient(typeof(IRepositoryBaseDbContext<,,>), typeof(RepositoryBaseDbContext<,,>));
        }

        public static OptionsBuilder<SqlServerRetryOptions> ConfigureSqlServerRetryOptionsPersistence(this IServiceCollection services, IConfiguration section)
        {
            // Read more: options pattern - https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0#bind-hierarchical-configuration
            // OptionsBuilder API
            // Options validation
            return services
                .AddOptions<SqlServerRetryOptions>()
                .Bind(section)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
    }
}