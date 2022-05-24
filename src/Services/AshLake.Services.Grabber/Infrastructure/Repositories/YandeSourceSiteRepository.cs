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

    public async Task<BsonDocument> GetLatestPostAsync()
    {
        var json = await _httpClient.GetStringAsync("/post.json?limit=1");
        Guard.Against.NullOrEmpty(json);

        var bson = JsonSerializer.DeserializeArray(json).First().AsDocument;
        return bson;
    }

    public async Task<BsonDocument?> GetMetadataAsync(int id, bool cachedEnable = true)
    {
        string tags = $"id:{id}";
        var cache = await _cachingProvider.GetAsync($"{id}",async () => (await GetMetadataListAsync(tags, 1, 1,false)).FirstOrDefault(), _cacheExpiration);

        return cache.Value;
    }

    public async Task<IEnumerable<BsonDocument>> GetMetadataListAsync(string tags, int limit, int page, bool cachedEnable = true)
    {
        string urlEncoded = WebUtility.UrlEncode(tags ?? "order:id");

        var json = await _httpClient.GetStringAsync($"/post.json?tags={urlEncoded}&limit={limit}&page={page}");
        Guard.Against.NullOrEmpty(json);

        var list = JsonSerializer.DeserializeArray(json).Select(x=>x.AsDocument).ToList();
        if (list.Count == 0) return new List<BsonDocument>();

        if (!cachedEnable) return list;

        var dic = list!.ToDictionary(x => x[YandePostMetadataKeys.id]!.AsInt32.ToString(),
                                     x => x.AsDocument);
        await _cachingProvider.SetAllAsync(dic, _cacheExpiration);

        return list;
    }

    public async Task<IEnumerable<BsonDocument>> GetMetadataListAsync(int startId, int limit, int page, bool cachedEnable = true)
    {
        string tags = $"id:>={startId} order:id";

        return await GetMetadataListAsync(tags, limit, page, cachedEnable);
    }

    public async Task<ImageFile> GetPreviewAsync(int id)
    {
        var metadata = await GetMetadataAsync(id);
        Guard.Against.Null(metadata, nameof(metadata));

        var status = metadata[YandePostMetadataKeys.status]?.AsString;
        Guard.Against.NullOrEmpty(status);
        var postStatus = Enum.Parse<PostStatus>(status.ToUpper());
        Guard.Against.InvalidInput(postStatus,
                                   YandePostMetadataKeys.status,
                                   x => x != PostStatus.DELETED);

        var previewUrl = metadata[YandePostMetadataKeys.preview_url]?.AsString;
        Guard.Against.NullOrEmpty(previewUrl);

        var md5 = metadata[YandePostMetadataKeys.md5]?.AsString;
        Guard.Against.NullOrEmpty(md5);

        var data = await _httpClient.GetStreamAsync(previewUrl);
        Guard.Against.Null(data, nameof(data));

        return new ImageFile(md5, ImageType.JPG, data);
    }

    public async Task<ImageFile> GetFileAsync(int id)
    {
        var metadata = await GetMetadataAsync(id);
        Guard.Against.Null(metadata, nameof(metadata));

        var status = metadata[YandePostMetadataKeys.status]?.AsString;
        Guard.Against.NullOrEmpty(status);
        var postStatus = Enum.Parse<PostStatus>(status.ToUpper());
        Guard.Against.InvalidInput(postStatus,
                                   YandePostMetadataKeys.status,
                                   x => x != PostStatus.DELETED);

        var fileUrl = metadata[YandePostMetadataKeys.file_url]?.AsString;
        Guard.Against.NullOrEmpty(fileUrl);

        var fileExt = metadata[YandePostMetadataKeys.file_ext]?.AsString;
        Guard.Against.NullOrEmpty(fileExt);
        var imagetType = Enum.Parse<ImageType>(fileExt.ToUpper());

        var md5 = metadata[YandePostMetadataKeys.md5]?.AsString;
        Guard.Against.NullOrEmpty(md5);

        var data = await _httpClient.GetStreamAsync(fileUrl);
        Guard.Against.Null(data, nameof(data));

        return new ImageFile(md5, imagetType, data);
    }
}
