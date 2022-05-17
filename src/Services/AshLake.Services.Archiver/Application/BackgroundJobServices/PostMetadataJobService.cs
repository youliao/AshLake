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


    //[Queue("metadata")]
    //public async Task<ArchiveStatus> AddOrUpdatePostMetadata(AddYandePostMetadataCommand command)
    //{
    //    var postMetadata = new PostMetadata(command.PostId.ToString(), BsonDocument.Parse(command.Data));
    //    var before = await _repository.FindOneAndReplaceAsync(postMetadata);

    //    if (before is null) return ArchiveStatus.Added;
    //    if (postMetadata.Equals(before)) return ArchiveStatus.Untouched;

    //    return ArchiveStatus.Updated;
    //}

    [Queue("metadata")]
    public async Task<int> AddOrUpdatePostMetadata(int startId, int limit)
    {
        var httpClient = _httpClientFactory.CreateClient(BooruSites.Yande);

        var serializeOptions = new JsonSerializerOptions();
        serializeOptions.Converters.Add(new BsonDocumentConverter());

        var metadataList = await httpClient.GetFromJsonAsync<List<BsonDocument>>($"sourcesites/yande/postmetadata?StartId={startId}&Page=1&Limit={limit}", serializeOptions);

        if (metadataList is null || metadataList.Count == 0) return 0;

        metadataList.RemoveAll(x => x["id"].AsInt32 >= startId + limit);

        foreach (var item in metadataList)
        {
            ArchiveStatus status;
            var postMetadata = new PostMetadata(item["id"].AsInt32.ToString(), item);
            var before = await _repository.FindOneAndReplaceAsync(postMetadata);

            if (before is null) 
            { 
                status = ArchiveStatus.Added;
                continue;
            }

            if (postMetadata.Equals(before))
            {
                status = ArchiveStatus.Untouched;
                continue;
            }

            status = ArchiveStatus.Updated;
        }

        return metadataList.Count;
    }
}
