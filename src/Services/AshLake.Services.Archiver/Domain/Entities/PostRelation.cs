using MongoDB.Bson.Serialization.Attributes;

namespace AshLake.Services.Archiver.Domain.Entities;

public record PostRelation
{
    /// <summary>
    /// Object key => MD5+EXT
    /// </summary>
    [BsonId]
    public string Id { get; init; } = default!;
    public bool? Exists { get; init; }
    public int? YandereId { get; init; }
    public int? DanbooruId { get; init; }
    public int? KonachanId { get; init; }
}
