namespace AshLake.Services.Grabber.Infrastructure.Repositories;

public class YandeSourceSiteRepository : IYandeSourceSiteRepository
{
    private readonly IEasyCachingProvider _cachingProvider;
    private readonly HttpClient _httpClient;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(1);

    public YandeSourceSiteRepository(IEasyCachingProvider cachingProvider, HttpClient httpClient)
    {
        _cachingProvider = cachingProvider;
        _httpClient = httpClient;
    }

    public async Task<JsonNode?> GetLatestPostAsync()
    {
        var list = await _httpClient.GetFromJsonAsync<IReadOnlyList<JsonNode>>($"/post.json?limit=1");
        if(list is null || list.Count ==0) return null;

        return list.FirstOrDefault();
    }

    public async Task<JsonNode?> GetMetadataAsync(int id, bool cachedEnable = true)
    {
        string tags = $"id:{id}";

        if (!cachedEnable) return (await GetMetadataListAsync(tags, 1, 1)).FirstOrDefault();

        var cache = await _cachingProvider.GetAsync($"{id}",
                                             async () => (await GetMetadataListAsync(tags,
                                                                                     1,
                                                                                     1,
                                                                                     false)).FirstOrDefault()?.ToString(),
                                             _cacheExpiration);

        if (cache.Value is null) return null;
        return JsonNode.Parse(cache.Value);
    }

    public async Task<IReadOnlyList<JsonNode>> GetMetadataListAsync(string tags, int limit, int page, bool cachedEnable = true)
    {
        string urlEncoded = WebUtility.UrlEncode(tags ?? "order:id");

        var list = await _httpClient.GetFromJsonAsync<IReadOnlyList<JsonNode>>($"/post.json?tags={urlEncoded}&limit={limit}&page={page}") ?? new List<JsonNode>();
        if (list.Count == 0) return list;
        if(!cachedEnable) return list;

        var dic = list!.ToDictionary(x => x[YandePostMetadataKeys.id]!.AsValue().ToString(),
                                     x => x.ToString());
        await _cachingProvider.SetAllAsync(dic, _cacheExpiration);

        return list;
    }

    public async Task<IReadOnlyList<JsonNode>> GetMetadataListAsync(int startId, int limit, int page, bool cachedEnable = true)
    {
        string tags = $"id:>={startId} order:id";

        return await GetMetadataListAsync(tags, limit, page, cachedEnable);
    }

    public async Task<ImageFile> GetPreviewAsync(int id)
    {
        var metadata = await GetMetadataAsync(id);
        Guard.Against.Null(metadata, nameof(metadata));

        var status = metadata[YandePostMetadataKeys.status]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(status);
        var postStatus = Enum.Parse<PostStatus>(status.ToUpper());
        Guard.Against.InvalidInput(postStatus,
                                   YandePostMetadataKeys.status,
                                   x => x != PostStatus.DELETED);

        var previewUrl = metadata[YandePostMetadataKeys.preview_url]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(previewUrl);

        var md5 = metadata[YandePostMetadataKeys.md5]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(md5);

        var data = await _httpClient.GetStreamAsync(previewUrl);
        Guard.Against.Null(data, nameof(data));

        return new ImageFile(md5, ImageType.JPG, data);
    }

    public async Task<ImageFile> GetFileAsync(int id)
    {
        var metadata = await GetMetadataAsync(id);
        Guard.Against.Null(metadata, nameof(metadata));

        var status = metadata[YandePostMetadataKeys.status]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(status);
        var postStatus = Enum.Parse<PostStatus>(status.ToUpper());
        Guard.Against.InvalidInput(postStatus,
                                   YandePostMetadataKeys.status,
                                   x => x != PostStatus.DELETED);

        var fileUrl = metadata[YandePostMetadataKeys.file_url]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(fileUrl);

        var fileExt = metadata[YandePostMetadataKeys.file_ext]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(fileExt);
        var imagetType = Enum.Parse<ImageType>(fileExt.ToUpper());

        var md5 = metadata[YandePostMetadataKeys.md5]?.AsValue()?.ToString();
        Guard.Against.NullOrEmpty(md5);

        var data = await _httpClient.GetStreamAsync(fileUrl);
        Guard.Against.Null(data, nameof(data));

        return new ImageFile(md5, imagetType, data);
    }
}
