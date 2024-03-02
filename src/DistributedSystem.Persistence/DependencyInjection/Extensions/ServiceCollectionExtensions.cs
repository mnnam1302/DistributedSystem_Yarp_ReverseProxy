using DistributedSystem.Domain.Abstractions;
using DistributedSystem.Domain.Abstractions.Repositories;
using DistributedSystem.Domain.Entities.Identity;
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

            // Cấu hình cho Identity
            services.AddIdentityCore<AppUser>(opt =>
            {
                // Default true - khóa tài khoản có được phép đối với người dùng mới hay không.
                opt.Lockout.AllowedForNewUsers = true; 
                // Default 2 minutes - tài khoản sẽ bị khóa sau một số lần đăng nhập không thành công.
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                // Default 5 -  có thể thử đăng nhập không thành công trước khi tài khoản của họ bị khóa.
                opt.Lockout.MaxFailedAccessAttempts = 3; 
            })
                .AddRoles<AppRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Cấu hình cho password
            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Password.RequireDigit = false; // Chữ số 0...9
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false; // Yêu cầu ít nhất một ký tự đặc biệt
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1; // Số lượng ký tự - mật khẩu cần phải bao gồm ít nhất một ký tự độc nhất.
                options.Lockout.AllowedForNewUsers = true;
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