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

    private IEasyCachingProvider _cachingProvider { get => _cachingProviderFactory.GetCachingProvider(nameof(Yande)); }

    public async Task<JsonObject?> GetMetadataAsync(int id, bool cachedEnable = true)
    {
        string tags = $"id:{id}";

        if (!cachedEnable) return (await GetMetadataListAsync(tags, 1, 1)).FirstOrDefault();

        var cache = await _cachingProvider.GetAsync($"{id}",
                                             async () => (await GetMetadataListAsync(tags, 1, 1)).FirstOrDefault(),
                                             TimeSpan.FromMinutes(10));

        return cache.Value;
    }

    public async Task<IReadOnlyList<JsonObject>> GetMetadataListAsync(string tags,int limit,int page)
    {
        string urlEncoded = WebUtility.UrlEncode(tags ?? "order:id");

        var list = await _httpClient.GetFromJsonAsync<IReadOnlyList<JsonObject>>($"/post.json?tags={urlEncoded}&limit={limit}&page={page}") ?? new List<JsonObject>();
        if (list.Count == 0) return list;

        var dic = list!.ToDictionary(x => x["id"]!.AsValue().ToString());
        await _cachingProvider.SetAllAsync(dic, TimeSpan.FromMinutes(10));

        return list;
    }

    public async Task<Stream> GetPreviewAsync(int id)
    {
        var metadata = await GetMetadataAsync(id);
        Guard.Against.Null(metadata,nameof(metadata));

        string previewUrlKey = "preview_url";
        var previewUrl = metadata[previewUrlKey]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(previewUrl, previewUrlKey);

        return await _httpClient.GetStreamAsync(previewUrl);
    }

    public async Task<(Stream, string)> GetFileAsync(int id)
    {
        var metadata = await GetMetadataAsync(id);
        Guard.Against.Null(metadata, nameof(metadata));

        string fileUrlKey = "file_url";
        var fileUrl = metadata[fileUrlKey]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(fileUrl, fileUrlKey);

        string fileExtKey = "file_ext";
        var fileExt = metadata[fileExtKey]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(fileExt, fileExtKey);

        var fileStream = await _httpClient.GetStreamAsync(fileUrl);
        Guard.Against.Null(fileStream, nameof(fileStream));

        return (fileStream, fileExt);
    }
}
