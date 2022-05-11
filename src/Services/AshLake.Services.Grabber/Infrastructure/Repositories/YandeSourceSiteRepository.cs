namespace AshLake.Services.Grabber.Infrastructure.Repositories;

public class YandeSourceSiteRepository
{
    private readonly IEasyCachingProvider _cachingProvider;
    private readonly IHttpClientFactory _httpClientFactory;

    public YandeSourceSiteRepository(IEasyCachingProvider cachingProvider, IHttpClientFactory httpClientFactory)
    {
        _cachingProvider = cachingProvider;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<JsonNode?> GetMetadataAsync(int id, bool cachedEnable = true)
    {
        string tags = $"id:{id}";

        if (!cachedEnable) return (await GetMetadataArrayAsync(tags, 1, 1)).FirstOrDefault();

        var cache = await _cachingProvider.GetAsync($"{BooruSites.Yande}:{id}",
                                             async () => (await GetMetadataArrayAsync(tags, 1, 1)).FirstOrDefault(),
                                             TimeSpan.FromMinutes(10));

        return cache.Value;
    }

    public async Task<JsonArray> GetMetadataArrayAsync(string tags,int limit,int page)
    {
        var client = _httpClientFactory.CreateClient(BooruSites.Yande);

        string urlEncoded = WebUtility.UrlEncode(tags ?? "order:id");

        return await client.GetFromJsonAsync<JsonArray>($"/post.json?tags={urlEncoded}&limit={limit}&page={page}") ?? new JsonArray();
    }

    public async Task<Stream> GetPreviewAsync(int id)
    {
        var client = _httpClientFactory.CreateClient(BooruSites.Yande);

        var metadata = await GetMetadataAsync(id);
        Guard.Against.Null(metadata,nameof(metadata));

        string key = "preview_url";
        var previewUrl = metadata[key]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(previewUrl, key);

        return await client.GetStreamAsync(previewUrl);
    }

    public async Task<Stream> GetFileAsync(int id)
    {
        var client = _httpClientFactory.CreateClient(BooruSites.Yande);
        client.Timeout = TimeSpan.FromMinutes(1);

        var metadata = await GetMetadataAsync(id);
        Guard.Against.Null(metadata, nameof(metadata));

        string key = "file_url";
        var previewUrl = metadata[key]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(previewUrl, key);

        return await client.GetStreamAsync(previewUrl);
    }
}
