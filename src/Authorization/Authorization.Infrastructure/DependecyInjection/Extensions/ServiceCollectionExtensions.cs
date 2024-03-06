using Authorization.Application.Abstractions;
using Authorization.Infrastructure.Authentication;
using Authorization.Infrastructure.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Infrastructure.DependecyInjection.Extensions
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
            services.AddStackExchangeRedisCache(redisOptions =>
            {
                var connectionString = configuration.GetConnectionString("Redis");
                redisOptions.Configuration = connectionString;
            });
        }
    }
}