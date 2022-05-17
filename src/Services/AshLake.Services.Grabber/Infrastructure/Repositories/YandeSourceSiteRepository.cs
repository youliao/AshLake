namespace AshLake.Services.Grabber.Infrastructure.Repositories;

public class YandeSourceSiteRepository
{
    private readonly IEasyCachingProviderFactory _cachingProviderFactory;
    private readonly HttpClient _httpClient;

    public YandeSourceSiteRepository(IEasyCachingProviderFactory cachingProviderFactory, HttpClient httpClient)
    {
        _cachingProviderFactory = cachingProviderFactory;
        _httpClient = httpClient;
    }

    private IEasyCachingProvider _cachingProvider { get => _cachingProviderFactory.GetCachingProvider(BooruSites.Yande); }

    public async Task<JsonNode?> GetMetadataAsync(int id, bool cachedEnable = true)
    {
        string tags = $"id:{id}";

        if (!cachedEnable) return (await GetMetadataListAsync(tags, 1, 1)).FirstOrDefault();

        var cache = await _cachingProvider.GetAsync($"{id}",
                                             async () => (await GetMetadataListAsync(tags, 1, 1)).FirstOrDefault(),
                                             TimeSpan.FromMinutes(10));

        return cache.Value;
    }

    public async Task<IReadOnlyList<JsonNode>> GetMetadataListAsync(string tags,int limit,int page)
    {
        string urlEncoded = WebUtility.UrlEncode(tags ?? "order:id");

        var list = await _httpClient.GetFromJsonAsync<IReadOnlyList<JsonNode>>($"/post.json?tags={urlEncoded}&limit={limit}&page={page}") ?? new List<JsonNode>();
        if (list.Count == 0) return list;

        var dic = list!.ToDictionary(x => x["id"]!.AsValue().ToString());
        await _cachingProvider.SetAllAsync(dic, TimeSpan.FromMinutes(10));

        return list;
    }

    public async Task<Stream> GetPreviewAsync(int id)
    {
        var metadata = await GetMetadataAsync(id);
        Guard.Against.Null(metadata,nameof(metadata));

        string key = "preview_url";
        var previewUrl = metadata[key]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(previewUrl, key);

        return await _httpClient.GetStreamAsync(previewUrl);
    }

    public async Task<Stream> GetFileAsync(int id)
    {
        var metadata = await GetMetadataAsync(id);
        Guard.Against.Null(metadata, nameof(metadata));

        string key = "file_url";
        var previewUrl = metadata[key]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(previewUrl, key);

        _httpClient.Timeout = TimeSpan.FromMinutes(1);
        return await _httpClient.GetStreamAsync(previewUrl);
    }
}
