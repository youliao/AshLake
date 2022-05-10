using System.Text.Json.Nodes;

namespace AshLake.Services.Grabber.Application.Yande.Queries;

public record GetYandePostMetadataQuery : IRequest<JsonNode?>
{
    public int Id { get; init; }
}

public class GetYandePostMetadataQueryHandler : IRequestHandler<GetYandePostMetadataQuery, JsonNode?>
{
    private readonly YandeSourceSiteRepository _repository;

    public GetYandePostMetadataQueryHandler(YandeSourceSiteRepository repository)
    {
        _repository = repository;
    }

    public async Task<JsonNode?> Handle(GetYandePostMetadataQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetMetadataAsync(query.Id);
    }
}