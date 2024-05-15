using MongoDB.Driver;
using Query.Domain.Abstractions.Options;
using Query.Domain.Attributes;

namespace Query.Persistence.Abstractions;

public abstract class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    protected MongoDbContext(IMongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<TDocument> GetCollection<TDocument>()
        => _database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));

    private static string GetCollectionName(Type documentType)
    {
        return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                typeof(BsonCollectionAttribute), true)
                .FirstOrDefault())?.CollectionName;
    }
}
