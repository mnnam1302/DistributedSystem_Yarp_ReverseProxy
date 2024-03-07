using ApiGateway.Abstractions;
using ApiGateway.Caching;

namespace ApiGateway.DependecyInjection.Extensions
{
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
    }
}
