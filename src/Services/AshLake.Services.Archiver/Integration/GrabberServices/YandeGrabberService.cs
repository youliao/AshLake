using MongoDB.Bson.Serialization;

namespace AshLake.Services.Archiver.Integration.GrabberServices;

public class YandeGrabberService : IYandeGrabberService
{
    private readonly HttpClient _httpClient;

    public YandeGrabberService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<IEnumerable<BsonDocument>> GetPostMetadataList(int startId, int limit)
    {
        var json = await _httpClient.GetStringAsync($"/api/sites/yande/postmetadata?StartId={startId}&Page=1&Limit={limit}");
        var list = BsonSerializer.Deserialize<BsonArray>(json)
            .Select(x => x.AsBsonDocument);

        return list ?? new List<BsonDocument>();
    }

    public async Task<PostPreview> GetPostPreview(int postId)
    {
        var response = await _httpClient.GetAsync($"/api/sites/yande/postpreviews/{postId}");
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

        return new PostPreview(postmd5, data);
    }

    public async Task<PostFile> GetPostFile(int postId)
    {
        var response = await _httpClient.GetAsync($"/api/sites/yande/postfiles/{postId}");
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
}
