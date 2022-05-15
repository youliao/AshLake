namespace AshLake.Services.Archiver.Application.Queries.GetPostMetadata;

public record GetPostMetadataQuery
{
    public string PostId { get; init; } = default!;
}

public abstract class GetPostMetadataQueryHandler<TQuery, TRepository>
    where TQuery : GetPostMetadataQuery
    where TRepository : IMetadataRepository<PostMetadata>
{
    private readonly TRepository _repository;

    public GetPostMetadataQueryHandler(TRepository repository)
    {
        _repository = repository;
    }

    public async Task<PostMetadata?> Handle(TQuery query, CancellationToken cancellationToken)
    {
        return await _repository.SingleAsync(query.PostId);
    }
}