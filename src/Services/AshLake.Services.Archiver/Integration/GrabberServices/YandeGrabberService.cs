namespace AshLake.Services.Archiver.Integration.GrabberServices;

public class YandeGrabberService : IGrabberService
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

        var result = await _httpClient.GetFromJsonAsync<List<BsonDocument>>($"/api/sites/yande/postmetadata?StartId={startId}&Page=1&Limit={limit}", serializeOptions);

        return result ?? new List<BsonDocument>();
    }
}
