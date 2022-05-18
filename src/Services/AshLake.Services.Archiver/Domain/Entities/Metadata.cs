using MongoDB.Bson.Serialization.Attributes;

namespace AshLake.Services.Archiver.Domain.Entities;

public abstract record Metadata
{
    [BsonId]
    public string Id { get => Data["id"]?.ToString() ?? throw new ArgumentNullException(nameof(Id)); }

    [BsonExtraElements]
    public BsonDocument Data { get; init; } = default!;

}
