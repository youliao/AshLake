namespace AshLake.Services.ArchiveBox.Application.Commands;

public record AddPostMetadataCommand : IRequest<Unit>
{
    public string Id { get; init; } = default!;
    public string Data { get; init; } = default!;
}

public class AddPostMetadataCommandHandler : IRequestHandler<AddPostMetadataCommand, Unit>
{
    private readonly IMetadataRepository<YandePostMetadata> _repository;

    public AddPostMetadataCommandHandler(IMetadataRepository<YandePostMetadata> repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(AddPostMetadataCommand command, CancellationToken cancellationToken)
    {
        var metadata = new YandePostMetadata(command.Id, BsonDocument.Parse(command.Data));
        await _repository.AddAsync(metadata);
        return Unit.Value;
    }
}
