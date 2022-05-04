namespace AshLake.Services.Yande.Application.Posts.Commands;

public record CreatePostCommand : IRequest<int>
{
    public string? Author { get; set; }
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
    public string? Source { get; set; }
    public PostStatus Status { get; set; }
    public List<string> Tags { get; set; } = null!;
    public DateTimeOffset UpdatedAt { get; set; }
    public int Width { get; set; }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreatePostCommand, int>
{
    private readonly IPostRepository _repository;

    public CreateTodoListCommandHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreatePostCommand command, CancellationToken cancellationToken)
    {
        var post = new Post(command.Author,
                            command.CreatedAt,
                            command.FileExt,
                            command.FileSize,
                            command.FileUrl,
                            command.HasChildren,
                            command.Height,
                            command.PostId,
                            command.Md5,
                            command.ParentId,
                            command.Rating,
                            command.Score,
                            command.Source,
                            command.Status,
                            command.Tags,
                            command.UpdatedAt,
                            command.Width);

       return await _repository.AddPostAsync(post);
    }
}

