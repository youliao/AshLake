using MongoDB.Bson.Serialization.Attributes;

namespace AshLake.Services.Archiver.Domain.Entities;

public record PostRelation
{
    /// <summary>
    /// MD5
    /// </summary>
    [BsonId]
    public string Id { get; init; } = default!;

    public int? YandereId { get; init; }
    public int? DanbooruId { get; init; }
    public int? KonachanId { get; init; }
}
