namespace AshLake.Services.Yande.Application.Posts;

public class PostCommandHandler
{
    private readonly IPostRepository _repository;
    private readonly IIntegrationEventBus _integrationEventBus;

    public PostCommandHandler(IPostRepository repository, IIntegrationEventBus integrationEventBus)
    {
        _repository = repository;
        _integrationEventBus = integrationEventBus;
    }

    [EventHandler]
    public async Task CreateHandleAsync(CreatePostCommand command)
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

        await _repository.AddPostAsync(post);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(DeletePostCommand command) =>
        await _repository.DeletePostAsync(command.PostId);
}