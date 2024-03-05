using Authorization.Application.Abstractions;
using Authorization.Infrastructure.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Infrastructure.DependecyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServicesInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IJwtTokenService, JwtTokenService>();
        }
    }
}