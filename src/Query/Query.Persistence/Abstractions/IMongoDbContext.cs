using MongoDB.Driver;

namespace Query.Persistence.Abstractions;

public interface IMongoDbContext
{
    IMongoCollection<TDocument> GetCollection<TDocument>();
}
