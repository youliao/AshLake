namespace AshLake.Services.YandeStore.Application.Posts.Commands;

public record UpdatePostCommand(string? Author,
                             DateTimeOffset CreatedAt,
                             string FileExt,
                             long FileSize,
                             string? FileUrl,
                             bool HasChildren,
                             int Height,
                             int PostId,
                             string Md5,
                             int? ParentId,
                             PostRating Rating,
                             int Score,
                             string? Source,
                             PostStatus Status,
                             List<string> Tags,
                             DateTimeOffset UpdatedAt,
                             int Width) : IRequest<int>;


public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, int>
{
    private readonly IPostRepository _repository;

    public UpdatePostCommandHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(UpdatePostCommand command, CancellationToken cancellationToken)
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

        return await _repository.AddOrUpdateAsync(post);
    }
}

