using MongoDB.Bson.Serialization.Attributes;

namespace AshLake.Services.Archiver.Domain.Entities;

public abstract record Metadata
{
    [BsonId]
    public int Id { get => Data["id"].AsInt32; }

    [BsonExtraElements]
    public BsonDocument Data { get; init; } = default!;

}
