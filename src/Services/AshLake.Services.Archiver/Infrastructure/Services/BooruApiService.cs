using MongoDB.Bson.Serialization;

namespace AshLake.Services.Archiver.Infrastructure.Services;

public interface IBooruApiService<T> where T : Booru
{
    Task<IEnumerable<BsonDocument>> GetPostMetadataList(int startId, int limit);

    Task<IEnumerable<BsonDocument>> GetTagMetadataList(int type);

    string GetPostFileDownloadLink(string objectKey);
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

    public string GetPostFileDownloadLink(string objectKey)
    {
        var link = new Uri(_httpClient.BaseAddress!, $"api/boorus/{_booru}/postfilelinks/{objectKey}");

        return link.ToString();
    }
}

public interface IBooruApiService
{
    string GetPostFileDownloadLink<T>(string objectKey) where T : Booru;
}

public class BooruApiService : IBooruApiService
{
    private readonly HttpClient _httpClient;

    public BooruApiService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public string GetPostFileDownloadLink<T>(string objectKey) where T : Booru
    {
        var _booru = typeof(T).Name.ToLower();
        var link = new Uri(_httpClient.BaseAddress!, $"api/boorus/{_booru}/postfilelinks/{objectKey}");

        return link.ToString();
    }
}