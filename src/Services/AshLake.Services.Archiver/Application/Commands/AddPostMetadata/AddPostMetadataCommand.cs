namespace AshLake.Services.Archiver.Application.Commands.AddPostMetadata;

public abstract record AddPostMetadataCommand
{
    public string PostId { get; init; } = default!;
    public string Data { get; init; } = default!;
}

public abstract class AddPostMetadataCommandHandler<TCommand, TRepository> 
    where TCommand : AddPostMetadataCommand 
    where TRepository : IMetadataRepository<PostMetadata>
{
    private readonly TRepository _repository;

    public AddPostMetadataCommandHandler(TRepository repository)
    {
        _repository = repository;
    }

    public async Task<ArchiveStatus> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var metadata = new PostMetadata(command.PostId, BsonDocument.Parse(command.Data));
        return await _repository.AddOrUpdateAsync(metadata);
    }
}
