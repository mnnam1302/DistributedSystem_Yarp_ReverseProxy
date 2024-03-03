using Microsoft.Extensions.DependencyInjection;

namespace Query.Application.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddMediatRApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly));

            return services;
        }

        //public static IServiceCollection AddAutoMapperApplication(this IServiceCollection services)
        //{
        //    services.AddAutoMapper(typeof(ServiceProfile));

        //    return services;
        //}
    }
}