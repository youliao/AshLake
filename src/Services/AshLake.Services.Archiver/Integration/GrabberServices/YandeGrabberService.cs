using MongoDB.Bson.Serialization;

namespace AshLake.Services.Archiver.Integration.GrabberServices;

public class YandeGrabberService : IYandeGrabberService
{
    private readonly HttpClient _httpClient;

    public YandeGrabberService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<BsonDocument>> GetPostMetadataList(int startId, int limit)
    {
        var serializeOptions = new JsonSerializerOptions();
        serializeOptions.Converters.Add(new BsonDocumentJsonConverter());

        var response = await _httpClient.GetFromJsonAsync<List<BsonDocument>>($"/api/sites/yande/postmetadata?StartId={startId}&Page=1&Limit={limit}", serializeOptions);

        return response ?? new List<BsonDocument>();


    }

    public async Task<string> GetPostPreview(int postId)
    {
        var bytes = await _httpClient.GetByteArrayAsync($"/api/sites/yande/postpreviews/{postId}");
        return Convert.ToBase64String(bytes);
    }

    public async Task<(string,string)> GetPostFile(int postId)
    {
        var response = await _httpClient.GetAsync($"/api/sites/yande/postfiles/{postId}");
        if(!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync());
        }

        var contentType = response.Content.Headers.ContentType?.ToString();
        Guard.Against.NullOrWhiteSpace(contentType);

        var fileExt = contentType.Split('/').Last();
        var allowedFileExt = new string[] { "jpg", "jpeg", "png", "gif" };
        if (!allowedFileExt.Contains(fileExt)) throw new FormatException($"FileExt must be {allowedFileExt}");

        var bytes = await response.Content.ReadAsByteArrayAsync();
        var base64Data = Convert.ToBase64String(bytes);

        return (base64Data, fileExt);
    }
}
