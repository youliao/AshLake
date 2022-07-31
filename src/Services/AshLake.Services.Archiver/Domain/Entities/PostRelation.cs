using MongoDB.Bson.Serialization.Attributes;

namespace AshLake.Services.Archiver.Domain.Entities;

public record PostRelation
{
    /// <summary>
    /// Object key => MD5+EXT
    /// </summary>
    [BsonId]
    public string Id { get; init; } = default!;
    public PostFileStatus? FileStatus { get; init; }
    public int? YandereId { get; init; }
    public int? DanbooruId { get; init; }
    public int? KonachanId { get; init; }
}

public enum PostFileStatus
{
    None = 0,
    Downloading = 1,
    InStock = 2,
    Invalid = 3,
    Deleted = 4,
    DownloadError = 5
}