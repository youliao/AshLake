namespace AshLake.Services.Yande.Application.Posts.Commands;

public record CreatePostCommand : Command
{
    public string Author { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string FileExt { get; set; } = null!;
    public long FileSize { get; set; }
    public string FileUrl { get; set; } = null!;
    public bool HasChildren { get; set; }
    public int Height { get; set; }
    public int PostId { get; set; }
    public string Md5 { get; set; } = null!;
    public int? ParentId { get; set; }
    public string Rating { get; set; } = null!;
    public int Score { get; set; }
    public string Source { get; set; }
    public PostStatus Status { get; set; }
    public List<string> Tags { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int Width { get; set; }
}
