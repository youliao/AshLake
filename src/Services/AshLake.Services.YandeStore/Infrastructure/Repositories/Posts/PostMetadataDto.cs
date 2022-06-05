namespace AshLake.Services.YandeStore.Infrastructure.Repositories.Posts;

public record PostMetadataDto(string? Author,
    IEnumerable<int>? ChildIds,
    DateTimeOffset CreatedAt,
    string FileExt,
    long FileSize,
    string? FileUrl,
    int Height,
    int Id,
    string Md5,
    int? ParentId,
    PostRating Rating,
    int Score,
    string? Source,
    PostStatus Status,
    IEnumerable<string> Tags,
    DateTimeOffset UpdatedAt,
    int Width);
