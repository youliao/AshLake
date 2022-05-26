namespace AshLake.Services.Yande.Application.Posts.Commands;

public record AddPostCommand(string? Author,
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
                             int Width) : IRequest<Unit>;


public class CreateTodoListCommandHandler : IRequestHandler<AddPostCommand, Unit>
{
    private readonly IPostRepository _repository;

    public CreateTodoListCommandHandler(IPostRepository repository)
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

