using MongoDB.Bson.Serialization;

namespace AshLake.Services.Archiver.Infrastructure.Services;

public class DanbooruGrabberService : IGrabberService<Danbooru>
{
    private readonly HttpClient _httpClient;

    public DanbooruGrabberService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<IEnumerable<BsonDocument>> GetPostMetadataList(int startId, int limit)
    {
        var json = await _httpClient.GetStringAsync($"/api/sites/danbooru/postmetadata?StartId={startId}&Page=1&Limit={limit}");
        var list = BsonSerializer.Deserialize<BsonArray>(json)
            .Select(x => x.AsBsonDocument);

        return list ?? new List<BsonDocument>();
    }

    public async Task<string?> GetPostObjectKey(int postId)
    {
        using var response = await _httpClient.GetAsync($"/api/sites/danbooru/postmetadata/{postId}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();

        var postmetadata = BsonSerializer.Deserialize<BsonDocument>(await response.Content.ReadAsStringAsync());

        var postmd5 = postmetadata[YandePostMetadataKeys.md5].AsString;
        Guard.Against.NullOrWhiteSpace(postmd5);

        var fileExt = postmetadata[YandePostMetadataKeys.file_ext].AsString;
        Guard.Against.NullOrWhiteSpace(fileExt);

        return $"{postmd5}.{fileExt}";
    }

    public async Task<IEnumerable<BsonDocument>> GetTagMetadataList(int type)
    {
        var json = await _httpClient.GetStringAsync($"/api/sites/danbooru/tagmetadata?Type={type}&Page=1&Limit=0");
        var list = BsonSerializer.Deserialize<BsonArray>(json)
            .Select(x => x.AsBsonDocument);

        return list ?? new List<BsonDocument>();
    }
}
