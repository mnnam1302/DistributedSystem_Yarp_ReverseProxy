using Query.Domain.Abstractions.Options;
using Query.Persistence.Abstractions;

namespace Query.Persistence;

public class ProjectionDbContext : MongoDbContext
{
    public ProjectionDbContext(IMongoDbSettings settings)
        : base(settings)
    {
    }
}
