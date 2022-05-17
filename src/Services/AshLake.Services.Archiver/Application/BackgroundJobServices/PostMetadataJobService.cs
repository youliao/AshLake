using AshLake.Services.Archiver.Application.Commands.AddPostMetadata;
using System.Text.Json;

namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class PostMetadataJobService 
{
    private readonly IYandeMetadataRepository<PostMetadata> _repository;
    private readonly IHttpClientFactory _httpClientFactory;

    public PostMetadataJobService(IYandeMetadataRepository<PostMetadata> repository, IHttpClientFactory httpClientFactory)
    {
        _repository = repository;
        _httpClientFactory = httpClientFactory;
    }

    [Queue("metadata")]
    public async Task<int> AddOrUpdatePostMetadata(int startId, int limit)
    {
        var httpClient = _httpClientFactory.CreateClient(BooruSites.Yande);

        var serializeOptions = new JsonSerializerOptions();
        serializeOptions.Converters.Add(new BsonDocumentJsonConverter());

        var metadataList = await httpClient.GetFromJsonAsync<List<BsonDocument>>($"sourcesites/yande/postmetadata?StartId={startId}&Page=1&Limit={limit}",
                                                                                 serializeOptions);

        if (metadataList is null || metadataList.Count == 0) return 0;

        metadataList.RemoveAll(x => x["id"].AsInt32 >= startId + limit);

        foreach (var item in metadataList)
        {
            var postMetadata = new PostMetadata(item["id"].AsInt32.ToString(), item);
            var status = await _repository.AddOrUpdateAsync(postMetadata);
        }

        return metadataList.Count;
    }
}
