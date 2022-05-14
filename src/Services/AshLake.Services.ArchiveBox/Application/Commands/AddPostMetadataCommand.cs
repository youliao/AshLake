namespace AshLake.Services.ArchiveBox.Application.Commands;

public record AddPostMetadataCommand : IRequest<ArchiveStatus>
{
    public string Id { get; init; } = default!;
    public string Data { get; init; } = default!;
}

public class AddPostMetadataCommandHandler : IRequestHandler<AddPostMetadataCommand, ArchiveStatus>
{
    private readonly IYandeMetadataRepository<PostMetadata> _repository;

    public AddPostMetadataCommandHandler(IYandeMetadataRepository<PostMetadata> repository)
    {
        _repository = repository;
    }

    public async Task<ArchiveStatus> Handle(AddPostMetadataCommand command, CancellationToken cancellationToken)
    {
        var metadata = new PostMetadata(command.Id, BsonDocument.Parse(command.Data));
        var before = await _repository.FindOneAndReplaceAsync(metadata);

        if (before is null) return ArchiveStatus.Added;
        if (metadata.Equals(before)) return ArchiveStatus.Untouched;

        return ArchiveStatus.Updated;
    }
}
