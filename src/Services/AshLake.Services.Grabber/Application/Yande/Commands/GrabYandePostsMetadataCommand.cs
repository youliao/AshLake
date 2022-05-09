namespace AshLake.Services.Grabber.Application.Yande.Commands;

public record GrabYandePostsMetadataCommand : IRequest<Unit>
{
    public int startId { get; init; }
    public int page { get; init; }
    public int limit { get; init; }
}

public class GrabYandePostsMetadataCommandHandler : IRequestHandler<GrabYandePostsMetadataCommand, Unit>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GrabYandePostsMetadataCommandHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Unit> Handle(GrabYandePostsMetadataCommand command, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(BooruSites.Yande);

        string tagsQuery = $"id:>={command.startId} order:id";
        string urlEncodedQuery = WebUtility.UrlEncode(tagsQuery);
        var result = await client.GetStringAsync($"/post.json?tags={urlEncodedQuery}&limit={command.limit}&page={command.page}");
        var bson = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(result);
        return Unit.Value;
    }
}