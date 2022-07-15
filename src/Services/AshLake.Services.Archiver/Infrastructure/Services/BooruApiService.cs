using MongoDB.Bson.Serialization;

namespace AshLake.Services.Archiver.Infrastructure.Services;

public interface IBooruApiService<T> where T : Booru
{
    Task<IEnumerable<BsonDocument>> GetPostMetadataList(int startId, int limit);

    Task<IEnumerable<BsonDocument>> GetTagMetadataList(int type);

    string GetPostFileLink(string objectKey);
}

public interface IBooruApiService
{
    Dictionary<string, string> GetPostFileLinks(string objectKey);
}

public class BooruApiService<T> : IBooruApiService<T> where T : Booru
{
    private readonly HttpClient _httpClient;
    private readonly string _booru = typeof(T).Name.ToLower();

    public BooruApiService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<IEnumerable<BsonDocument>> GetPostMetadataList(int start, int limit)
    {
        var json = await _httpClient.GetStringAsync($"/api/boorus/{_booru}/posts?start={start}&limit={limit}");
        var list = BsonSerializer.Deserialize<BsonArray>(json)
            .Select(x => x.AsBsonDocument);

        return list ?? new List<BsonDocument>();
    }

    public async Task<IEnumerable<BsonDocument>> GetTagMetadataList(int type)
    {
        var json = await _httpClient.GetStringAsync($"/api/boorus/{_booru}/tagmetadata?Type={type}&Page=1&Limit=0");
        var list = BsonSerializer.Deserialize<BsonArray>(json)
            .Select(x => x.AsBsonDocument);

        return list ?? new List<BsonDocument>();
    }

    public string GetPostFileLink(string objectKey)
    {
        var link = new Uri(_httpClient.BaseAddress!, $"api/boorus/{_booru}/postfilelinks/{objectKey}");

        return link.ToString();
    }
}

public class BooruApiService : IBooruApiService
{
    private readonly HttpClient _httpClient;

    public BooruApiService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public Dictionary<string,string> GetPostFileLinks(string objectKey)
    {
        var baseAddress = _httpClient.BaseAddress!;

        var yandereUri = new Uri(baseAddress, $"api/boorus/{Yandere.Alias}/postfilelinks/{objectKey}");
        var danbooruUri = new Uri(baseAddress, $"api/boorus/{Danbooru.Alias}/postfilelinks/{objectKey}");
        var konachanUri = new Uri(baseAddress, $"api/boorus/{Konachan.Alias}/postfilelinks/{objectKey}");
        var gelbooruUri = new Uri(baseAddress, $"api/boorus/{Gelbooru.Alias}/postfilelinks/{objectKey}");

        return new Dictionary<string, string>
        {
            {nameof(Yandere),yandereUri.ToString()},
            {nameof(Danbooru),danbooruUri.ToString()},
            {nameof(Konachan),konachanUri.ToString()},
            {nameof(Gelbooru),gelbooruUri.ToString()},
        };
    }
}
