using System.Text.Json.Nodes;

namespace AshLake.Services.Grabber.Application.Yande.Queries;

public record GetYandePostsMetadataArrayQuery : IRequest<JsonArray>
{
    public int StartId { get; init; }
    public int Page { get; init; }
    public int Limit { get; init; }
}

public class GetYandePostsMetadataArrayQueryHandler : IRequestHandler<GetYandePostsMetadataArrayQuery, JsonArray>
{
    private readonly YandeSourceSiteRepository _repository;

    public GetYandePostsMetadataArrayQueryHandler(YandeSourceSiteRepository repository)
    {
        _repository = repository;
    }

    public async Task<JsonArray> Handle(GetYandePostsMetadataArrayQuery query, CancellationToken cancellationToken)
    {
        string tags = $"id:>={query.StartId} order:id";

        return await _repository.GetMetadataArrayAsync(tags, query.Limit, query.Page);
    }
}