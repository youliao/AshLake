namespace AshLake.Services.Yande.Domain.Posts;

public class Post
{
    public string? Author { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string FileExt { get; private set; } = null!;
    public long FileSize { get; private set; }
    public string? FileUrl { get; private set; } = null!;
    public bool HasChildren { get; private set; }
    public int Height { get; private set; }
    public int Id { get; private set; }
    public string Md5 { get; private set; } = null!;
    public int? ParentId { get; private set; }
    public PostRating Rating { get; private set; }
    public int Score { get; private set; }
    public string? Source { get; private set; }
    public PostStatus Status { get; private set; }
    public List<string> Tags { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public int Width { get; private set; }

    public Post(string? author,
                DateTimeOffset createdAt,
                string fileExt,
                long fileSize,
                string? fileUrl,
                bool hasChildren,
                int height,
                int id,
                string md5,
                int? parentId,
                PostRating rating,
                int score,
                string? source,
                PostStatus status,
                List<string> tags,
                DateTimeOffset updatedAt,
                int width)
    {
        Author = author;
        CreatedAt = createdAt;
        FileExt = fileExt;
        FileSize = fileSize;
        FileUrl = fileUrl;
        HasChildren = hasChildren;
        Height = height;
        Id = id;
        Md5 = md5;
        ParentId = parentId;
        Rating = rating;
        Score = score;
        Source = source;
        Status = status;
        Tags = tags;
        UpdatedAt = updatedAt;
        Width = width;
    }

    public void Delete()
    {
        Status = PostStatus.DELETED;
    }
}
