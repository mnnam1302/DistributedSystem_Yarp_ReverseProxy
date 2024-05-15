using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Query.Domain.Abstractions.Options;
using Query.Domain.Abstractions.Repositories;
using Query.Persistence.Abstractions;
using Query.Persistence.DependencyInjection.Options;
using Query.Persistence.Repositories;

namespace Query.Persistence.DependencyInjection.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServicesPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));

        services.AddSingleton<IMongoDbSettings>(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

        services.AddScoped<IMongoDbContext, ProjectionDbContext>();
        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

        return services;
    }
}
