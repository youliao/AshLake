namespace AshLake.Services.YandeStore.Application.Posts.Commands;

public record BulkUpdatePostsCommand(IEnumerable<AddOrUpdatePostCommand> AddOrUpdatePostCommands) : IRequest<int>;


public class BulkUpdatePostsCommandHandler : IRequestHandler<BulkUpdatePostsCommand, int>
{
    private readonly IPostRepository _repository;

    public BulkUpdatePostsCommandHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(BulkUpdatePostsCommand command, CancellationToken cancellationToken)
    {
        var posts = command.AddOrUpdatePostCommands
            .Select(x => new Post(x.Author,
                                  x.CreatedAt,
                                  x.FileExt,
                                  x.FileSize,
                                  x.FileUrl,
                                  x.HasChildren,
                                  x.Height,
                                  x.PostId,
                                  x.Md5,
                                  x.ParentId,
                                  x.Rating,
                                  x.Score,
                                  x.Source,
                                  x.Status,
                                  x.Tags,
                                  x.UpdatedAt,
                                  x.Width));

        return await _repository.UpdateRangeAsync(posts);
    }
}

