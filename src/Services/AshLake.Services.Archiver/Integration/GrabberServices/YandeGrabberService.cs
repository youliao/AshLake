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
        var response = await _httpClient.GetByteArrayAsync($"/api/sites/yande/postpreviews/{postId}");
        return Convert.ToBase64String(response);
    }
}
