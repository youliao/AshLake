using MongoDB.Bson.Serialization;

namespace AshLake.Services.YandeStore.Infrastructure.Services;

public interface IYandeArchiverService
{
    Task<BsonDocument> GetPostMetadata(int id);
    Task<IEnumerable<BsonDocument>> GetPostMetadataByRange(int rangeFrom, int rangeTo);
}

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

    public async Task<IEnumerable<BsonDocument>> GetPostMetadataByRange(int rangeFrom, int rangeTo)
    {
        var json = await _httpClient.GetStringAsync($"/api/sites/yande/postmetadata?rangeFrom={rangeFrom}&rangeTo={rangeTo}");
        var list = BsonSerializer.Deserialize<BsonArray>(json).Select(x => x.AsBsonDocument);
        return list;
    }

    //public async Task<IEnumerable<BsonDocument>> GetPostMetadataByIds(IEnumerable<int> ids)
    //{
    //    var json = await _httpClient.GetStringAsync($"/api/sites/yande/postmetadata?ids={string.Join(',', ids)}");
    //    var list = BsonSerializer.Deserialize<BsonArray>(json).Select(x => x.AsBsonDocument);
    //    return list;
    //}
}
