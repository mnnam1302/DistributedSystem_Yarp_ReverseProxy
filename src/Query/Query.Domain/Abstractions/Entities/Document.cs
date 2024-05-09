using MongoDB.Bson;

namespace Query.Domain.Abstractions.Entities;

public abstract class Document : IDocument
{
    public ObjectId Id { get; set; }

    public Guid DocumentId { get; set; }

    public DateTimeOffset CreatedOnUtc => Id.CreationTime;

    public DateTimeOffset? ModifiedOnUtc { get; set; }
}
