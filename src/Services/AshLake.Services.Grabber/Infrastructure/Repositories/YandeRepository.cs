namespace AshLake.Services.Grabber.Infrastructure.Repositories;

public class YandeRepository
{
    private readonly IHttpClientFactory _httpClientFactory;

    public YandeRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<JsonArray> GetMetadataListAsync(string tags,int limit,int page)
    {
        var client = _httpClientFactory.CreateClient(BooruSites.Yande);

        string urlEncoded = WebUtility.UrlEncode(tags ?? "order:id");

        return await client.GetFromJsonAsync<JsonArray>($"/post.json?tags={urlEncoded}&limit={limit}&page={page}") ?? new JsonArray();
    }
}
