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

    public async Task<byte[]> GetPostPreview(int postId)
    {
        return await _httpClient.GetByteArrayAsync($"/api/sites/yande/postpreviews/{postId}");
    }

    public async Task<(byte[], string)> GetPostFile(int postId)
    {
        var response = await _httpClient.GetAsync($"/api/sites/yande/postfiles/{postId}");
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync());
        }

        var contentType = response.Content.Headers.ContentType?.ToString();
        Guard.Against.NullOrWhiteSpace(contentType);

        var fileExt = contentType.Split('/').Last();
        var allowedFileExt = new string[] { "jpg", "jpeg", "png", "gif" };
        if (!allowedFileExt.Contains(fileExt)) throw new FormatException($"FileExt must be {allowedFileExt}");

        var bytes = await response.Content.ReadAsByteArrayAsync();
        return (bytes, fileExt);
    }
}
