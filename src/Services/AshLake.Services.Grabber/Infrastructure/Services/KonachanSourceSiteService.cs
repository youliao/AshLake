namespace AshLake.Services.Grabber.Infrastructure.Services;

public interface IKonachanSourceSiteService
{
    Task<ImageFile> GetFileAsync(int id);
    Task<ImageFile> GetPreviewAsync(int id);
    Task<JToken> GetLatestPostMetadataAsync();
    Task<JToken?> GetPostMetadataAsync(int id);
    Task<IEnumerable<JToken>> GetPostMetadataListAsync(string tags, int limit, int page);
    Task<IEnumerable<JToken>> GetPostMetadataListAsync(int startId, int limit, int page);
    Task<IEnumerable<JToken>> GetTagMetadataListAsync(int? type, int limit, int page);
}

public class KonachanSourceSiteService : IKonachanSourceSiteService
{
    private readonly IEasyCachingProvider _cachingProvider;
    private readonly HttpClient _httpClient;

    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

    public KonachanSourceSiteService(IEasyCachingProviderFactory factory, HttpClient httpClient)
    {
        _cachingProvider = factory.GetCachingProvider(nameof(Yande));
        _httpClient = httpClient;
    }

    public async Task<JToken> GetLatestPostMetadataAsync()
    {
        var json = await _httpClient.GetStringAsync("/post.json?limit=1");
        Guard.Against.NullOrEmpty(json);

        return JArray.Parse(json).First!;
    }

    public async Task<JToken?> GetPostMetadataAsync(int id)
    {
        var idStr = id.ToString();
        var cache = await _cachingProvider.GetAsync<JToken>(idStr);
        if (cache.HasValue) return cache.Value;

        var list = await GetPostMetadataListAsync(id - 50, 100, 1);
        if (list.Count() == 0) return null;

        var dic = list.ToDictionary(x => x.Value<string>(KonachanPostMetadataKeys.id)!, x => x);

        await _cachingProvider.SetAllAsync(dic, _cacheExpiration);

        var first = list.First();
        if (first[KonachanPostMetadataKeys.id]!.ToString() != idStr) return null;

        return first;
    }

    public async Task<IEnumerable<JToken>> GetPostMetadataListAsync(string tags, int limit, int page)
    {
        Guard.Against.OutOfRange(limit, nameof(limit), 1, 1000);
        string urlEncoded = WebUtility.UrlEncode(tags ?? "order:id");

        var json = await _httpClient.GetStringAsync($"/post.json?tags={urlEncoded}&limit={limit}&page={page}");
        Guard.Against.NullOrEmpty(json);

        var list = JArray.Parse(json).Where(x => x.Value<string>(KonachanPostMetadataKeys.id) != null);

        return list;
    }

    public async Task<IEnumerable<JToken>> GetPostMetadataListAsync(int startId, int limit, int page)
    {
        string tags = $"id:>={startId} order:id";

        return await GetPostMetadataListAsync(tags, limit, page);
    }

    public async Task<ImageFile> GetPreviewAsync(int id)
    {
        var metadata = await GetPostMetadataAsync(id);
        Guard.Against.Null(metadata, nameof(metadata));

        var status = metadata[KonachanPostMetadataKeys.status]?.ToString();
        Guard.Against.NullOrEmpty(status);
        var postStatus = Enum.Parse<PostStatus>(status.ToUpper());
        Guard.Against.InvalidInput(postStatus,
                                   KonachanPostMetadataKeys.status,
                                   x => x != PostStatus.DELETED);

        var previewUrl = metadata[KonachanPostMetadataKeys.preview_url]?.ToString();
        Guard.Against.NullOrEmpty(previewUrl);

        var md5 = metadata[KonachanPostMetadataKeys.md5]?.ToString();
        Guard.Against.NullOrEmpty(md5);

        var data = await _httpClient.GetByteArrayAsync(previewUrl);
        Guard.Against.Null(data, nameof(data));

        return new ImageFile(md5, ImageType.JPG, data);
    }

    public async Task<ImageFile> GetFileAsync(int id)
    {
        var metadata = await GetPostMetadataAsync(id);
        Guard.Against.Null(metadata, nameof(metadata));

        var status = metadata[KonachanPostMetadataKeys.status]?.ToString();
        Guard.Against.NullOrEmpty(status);
        var postStatus = Enum.Parse<PostStatus>(status.ToUpper());
        Guard.Against.InvalidInput(postStatus,
                                   KonachanPostMetadataKeys.status,
                                   x => x != PostStatus.DELETED);

        var fileUrl = metadata[KonachanPostMetadataKeys.file_url]?.ToString();
        Guard.Against.NullOrEmpty(fileUrl);

        var fileExt = Path.GetExtension(fileUrl).TrimStart('.');
        Guard.Against.NullOrEmpty(fileExt);
        var imagetType = Enum.Parse<ImageType>(fileExt.ToUpper());

        var md5 = metadata[KonachanPostMetadataKeys.md5]?.ToString();
        Guard.Against.NullOrEmpty(md5);

        var data = await _httpClient.GetByteArrayAsync(fileUrl);
        Guard.Against.Null(data, nameof(data));

        return new ImageFile(md5, imagetType, data);
    }

    public async Task<IEnumerable<JToken>> GetTagMetadataListAsync(int? type,int limit, int page)
    {
        var json = await _httpClient.GetStringAsync($"/tag.json?order=date&limit={limit}&page={page}&type={type}");
        Guard.Against.NullOrEmpty(json);

        return JArray.Parse(json).ToList();
    }
}
