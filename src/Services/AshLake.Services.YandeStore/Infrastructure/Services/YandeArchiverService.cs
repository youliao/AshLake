using AshLake.Services.YandeStore.Application.Services;

namespace AshLake.Services.YandeStore.Infrastructure.Services;

public class YandeArchiverService : IYandeArchiverService
{
    private readonly HttpClient _httpClient;

    public YandeArchiverService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<BsonDocument> GetPostMetadata(int id)
    {
        var json = await _httpClient.GetStringAsync($"/api/sites/yande/postmetadata/{id}");
        return BsonDocument.Parse(json);
    }
}
