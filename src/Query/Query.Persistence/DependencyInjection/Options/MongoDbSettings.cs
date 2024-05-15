using Query.Domain.Abstractions.Options;

namespace Query.Persistence.DependencyInjection.Options;

public class MongoDbSettings : IMongoDbSettings
{
    public string ConnectionString { get; set; }

    public string DatabaseName { get; set; }
}
