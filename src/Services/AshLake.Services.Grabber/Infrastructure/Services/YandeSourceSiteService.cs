﻿namespace AshLake.Services.Grabber.Infrastructure.Services;

public interface IYandeSourceSiteService
{
    Task<Stream> GetFileAsync(int id);
    Task<string> GetFileUrlAsync(int id);
    Task<ImageFile> GetPreviewAsync(int id);
    Task<JToken> GetLatestPostMetadataAsync();
    Task<JToken?> GetPostMetadataAsync(int id);
    Task<IEnumerable<JToken>> GetPostMetadataListAsync(string tags, int limit, int page);
    Task<IEnumerable<JToken>> GetPostMetadataListAsync(int startId, int limit, int page);
    Task<IEnumerable<JToken>> GetTagMetadataListAsync(int? type, int limit, int page);
}

public class YandeSourceSiteService : IYandeSourceSiteService
{
    private readonly IEasyCachingProvider _cachingProvider;
    private readonly HttpClient _httpClient;

    private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(1);

    public YandeSourceSiteService(IEasyCachingProviderFactory factory, HttpClient httpClient)
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

        var dic = list.ToDictionary(x => x.Value<string>(YandePostMetadataKeys.id)!, x => x);

        await _cachingProvider.SetAllAsync(dic, _cacheExpiration);

        return list.SingleOrDefault(x => x.Value<string>(YandePostMetadataKeys.id) == idStr);
    }

    public async Task<IEnumerable<JToken>> GetPostMetadataListAsync(string tags, int limit, int page)
    {
        Guard.Against.OutOfRange(limit, nameof(limit), 1, 1000);
        string urlEncoded = WebUtility.UrlEncode(tags ?? "order:id");

        var json = await _httpClient.GetStringAsync($"/post.json?tags={urlEncoded}&limit={limit}&page={page}");
        Guard.Against.NullOrEmpty(json);

        var list = JArray.Parse(json).Where(x => x.Value<string>(YandePostMetadataKeys.id) != null);

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

        var status = metadata[YandePostMetadataKeys.status]?.ToString();
        Guard.Against.NullOrEmpty(status);
        var postStatus = Enum.Parse<PostStatus>(status.ToUpper());
        Guard.Against.InvalidInput(postStatus,
                                   YandePostMetadataKeys.status,
                                   x => x != PostStatus.DELETED);

        var previewUrl = metadata[YandePostMetadataKeys.preview_url]?.ToString();
        Guard.Against.NullOrEmpty(previewUrl);

        var md5 = metadata[YandePostMetadataKeys.md5]?.ToString();
        Guard.Against.NullOrEmpty(md5);

        var data = await _httpClient.GetByteArrayAsync(previewUrl);
        Guard.Against.Null(data, nameof(data));

        return new ImageFile(md5, ImageType.JPG, data);
    }

    public async Task<Stream> GetFileAsync(int id)
    {
        var metadata = await GetPostMetadataAsync(id);
        Guard.Against.Null(metadata, nameof(metadata));

        var status = metadata[YandePostMetadataKeys.status]?.ToString();
        Guard.Against.NullOrEmpty(status);
        var postStatus = Enum.Parse<PostStatus>(status.ToUpper());
        Guard.Against.InvalidInput(postStatus,
                                   YandePostMetadataKeys.status,
                                   x => x != PostStatus.DELETED);

        var fileUrl = metadata[YandePostMetadataKeys.file_url]?.ToString();
        Guard.Against.NullOrEmpty(fileUrl);

        var stream = await _httpClient.GetStreamAsync(fileUrl);

        return stream;
    }

    public async Task<string> GetFileUrlAsync(int id)
    {
        var metadata = await GetPostMetadataAsync(id);
        Guard.Against.Null(metadata, nameof(metadata));

        var status = metadata[YandePostMetadataKeys.status]?.ToString();
        Guard.Against.NullOrEmpty(status);
        var postStatus = Enum.Parse<PostStatus>(status.ToUpper());
        Guard.Against.InvalidInput(postStatus,
                                   YandePostMetadataKeys.status,
                                   x => x != PostStatus.DELETED);

        var fileUrl = metadata[YandePostMetadataKeys.file_url]?.ToString();
        Guard.Against.NullOrEmpty(fileUrl);

        //using var httpRequest = new HttpRequestMessage(HttpMethod.Head, fileUrl);
        //using var httpResponse = await _httpClient.SendAsync(httpRequest);
        //httpResponse.EnsureSuccessStatusCode();

        return fileUrl;
    }

    public async Task<IEnumerable<JToken>> GetTagMetadataListAsync(int? type,int limit, int page)
    {
        var json = await _httpClient.GetStringAsync($"/tag.json?order=date&limit={limit}&page={page}&type={type}");
        Guard.Against.NullOrEmpty(json);

        return JArray.Parse(json).ToList();
    }
}
