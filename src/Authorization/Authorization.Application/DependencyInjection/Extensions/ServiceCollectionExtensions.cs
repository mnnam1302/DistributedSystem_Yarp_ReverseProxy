using DistributedSystem.Application.Mapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.Contracts;

namespace Authorization.Application.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMediatRApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(AssemblyReference.Assembly))
                .AddValidatorsFromAssembly(DistributedSystem.Contract.AssemblyReference.Assembly, includeInternalTypes: true);

        }

        public static void AddAutoMapperApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ServiceProfile));
        }
    }
}