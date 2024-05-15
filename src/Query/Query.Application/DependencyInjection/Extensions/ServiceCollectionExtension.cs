using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Services.V1.Product;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Query.Application.Behaviors;
using Query.Application.Mapper;
using Query.Application.UseCases.V1.Queries.Product;
using static DistributedSystem.Contract.Services.V1.Product.Query;

namespace Query.Application.DependencyInjection.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddMediatRApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformancePipelineBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(TracingPipelineBehavior<,>));

        return services;
    }

    public static IServiceCollection AddAutoMapperApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ServiceProfile));

        return services;
    }
}
