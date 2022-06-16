using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace AshLake.Services.Collector.Infrastructure.Services;

public interface IDanbooruService
{
    Task<PostFile?> GetPostFile(int postId);

    Task<string?> GetPostObjectKey(int postId);
}

public class DanbooruGrabberService : IDanbooruService
{
    private readonly HttpClient _httpClient;

    public DanbooruGrabberService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<PostFile?> GetPostFile(int postId)
    {
        using var response = await _httpClient.GetAsync($"/api/sites/danbooru/postfiles/{postId}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();

        var contentType = response.Content.Headers.ContentType?.ToString();
        Guard.Against.NullOrWhiteSpace(contentType);

        var fileExt = contentType?.Split('/').LastOrDefault();
        Guard.Against.NullOrWhiteSpace(fileExt);
        var imageType = Enum.Parse<ImageType>(fileExt.ToUpper());

        var postmd5 = response.Headers?.GetValues("X-MD5").FirstOrDefault();
        Guard.Against.NullOrWhiteSpace(postmd5);

        var data = await response.Content.ReadAsByteArrayAsync();
        Guard.Against.Null(data);

        return new PostFile(postmd5, imageType, data);
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

        var postmd5 = postmetadata[DanbooruPostMetadataKeys.md5].AsString;
        Guard.Against.NullOrWhiteSpace(postmd5);

        var fileExt = postmetadata[DanbooruPostMetadataKeys.file_ext].AsString;
        Guard.Against.NullOrWhiteSpace(fileExt);

        return $"{postmd5}.{fileExt}";
    }
}
