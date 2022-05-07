namespace AshLake.Services.Yande.Application.Posts.Commands;

public record AddPostCommand : IRequest<Unit>
{
    public string? Author { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public string FileExt { get; init; } = null!;
    public long FileSize { get; init; }
    public string FileUrl { get; init; } = null!;
    public bool HasChildren { get; init; }
    public int Height { get; init; }
    public int PostId { get; init; }
    public string Md5 { get; init; } = null!;
    public int? ParentId { get; init; }
    public string Rating { get; init; } = null!;
    public int Score { get; init; }
    public string? Source { get; init; }
    public PostStatus Status { get; init; }
    public List<string> Tags { get; init; } = null!;
    public DateTimeOffset UpdatedAt { get; init; }
    public int Width { get; init; }
}

public class AddPostCommandHandler : IRequestHandler<AddPostCommand, Unit>
{
    private readonly IPostRepository _repository;

    public AddPostCommandHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(AddPostCommand command, CancellationToken cancellationToken)
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

        await _repository.AddAsync(post);
        return Unit.Value;
    }
}

