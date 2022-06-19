using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace AshLake.Services.Collector.Infrastructure.Services;

public interface IArchiverService<T> where T : ISouceSite
{
    Task<string?> GetPostObjectKey(int postId);
}

public class ArchiverService<T> : IArchiverService<T> where T : ISouceSite
{
    private readonly HttpClient _httpClient;

    private readonly string _souceSiteName;

    public ArchiverService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _souceSiteName = typeof(T).Name.ToLower();
    }

    public async Task<string?> GetPostObjectKey(int postId)
    {
        using var response = await _httpClient.GetAsync($"/api/sites/{_souceSiteName}/postmetadata/{postId}");
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
}
