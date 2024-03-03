using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Query.Domain.Abstractions.Entities
{
    public interface IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        ObjectId Id { get; set; }

        DateTimeOffset CreatedOnUtc { get; }

        DateTimeOffset? ModifiedOnUtc { get; }
    }
}