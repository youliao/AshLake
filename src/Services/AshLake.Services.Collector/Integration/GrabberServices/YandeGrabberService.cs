using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace AshLake.Services.Collector.Integration.GrabberServices;

public class YandeGrabberService : IYandeGrabberService
{
    private readonly HttpClient _httpClient;

    public YandeGrabberService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<PostPreview?> GetPostPreview(int postId)
    {
        var response = await _httpClient.GetAsync($"/api/sites/yande/postpreviews/{postId}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync());
        }

        var contentType = response.Content.Headers.ContentType?.ToString();
        Guard.Against.NullOrWhiteSpace(contentType);

        var fileExt = contentType?.Split('/').LastOrDefault();
        Guard.Against.NullOrWhiteSpace(fileExt);
        Enum.Parse<ImageType>(fileExt.ToUpper());

        var postmd5 = response.Headers?.GetValues("postmd5").FirstOrDefault();
        Guard.Against.NullOrWhiteSpace(postmd5);

        var data = await response.Content.ReadAsByteArrayAsync();
        Guard.Against.Null(data);

        return new PostPreview(postmd5, data);
    }

    public async Task<PostFile?> GetPostFile(int postId)
    {
        var response = await _httpClient.GetAsync($"/api/sites/yande/postfiles/{postId}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync());
        }

        var contentType = response.Content.Headers.ContentType?.ToString();
        Guard.Against.NullOrWhiteSpace(contentType);

        var fileExt = contentType?.Split('/').LastOrDefault();
        Guard.Against.NullOrWhiteSpace(fileExt);
        var imageType = Enum.Parse<ImageType>(fileExt.ToUpper());

        var postmd5 = response.Headers?.GetValues("postmd5").FirstOrDefault();
        Guard.Against.NullOrWhiteSpace(postmd5);

        var data = await response.Content.ReadAsByteArrayAsync();
        Guard.Against.Null(data);

        return new PostFile(postmd5, imageType, data);
    }

    public async Task<string?> GetPostObjectKey(int postId)
    {
        var response = await _httpClient.GetAsync($"/api/sites/yande/postmetadata/{postId}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync());
        }

        var postmetadata = BsonSerializer.Deserialize<BsonDocument>(await response.Content.ReadAsStringAsync());

        var postmd5 = postmetadata[YandePostMetadataKeys.md5].AsString;
        Guard.Against.NullOrWhiteSpace(postmd5);

        var fileExt = postmetadata[YandePostMetadataKeys.file_ext].AsString;
        Guard.Against.NullOrWhiteSpace(fileExt);

        return $"{postmd5}.{fileExt}";
    }
}
