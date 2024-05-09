using Microsoft.Extensions.DependencyInjection;
using Query.Domain.Abstractions.Repositories;
using Query.Persistence.Repositories;

namespace Query.Persistence.DependencyInjection.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddServicesPersistence(this IServiceCollection services)
    {
        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
    }
}
