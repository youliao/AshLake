using System.Text.Json.Nodes;

namespace AshLake.Services.Grabber.Application.Yande.Queries;

public record GetYandePostsMetadataQuery : IRequest<JsonArray>
{
    public int startId { get; init; }
    public int page { get; init; }
    public int limit { get; init; }
}

public class GetYandePostsMetadataQueryHandler : IRequestHandler<GetYandePostsMetadataQuery, JsonArray>
{
    private readonly YandeRepository _repository;

    public GetYandePostsMetadataQueryHandler(YandeRepository repository)
    {
        _repository = repository;
    }

    public async Task<JsonArray> Handle(GetYandePostsMetadataQuery query, CancellationToken cancellationToken)
    {
        string tags = $"id:>={query.startId} order:id";

        return await _repository.GetMetadataListAsync(tags, query.limit, query.page);
    }
}