using MongoDB.Driver;

namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class PostMetadataJob<T> where T : Booru
{
    private readonly IMetadataRepository<T, PostMetadata> _postMetadataRepository;
    private readonly IBooruApiService<T> _grabberService;
    private readonly IPublishEndpoint _publishEndpoint;

    public PostMetadataJob(IMetadataRepository<T, PostMetadata> postMetadataRepository, IBooruApiService<T> grabberService, IPublishEndpoint publishEndpoint)
    {
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    [Queue("{0}")]
    [AutomaticRetry(Attempts = 3)]
    public async Task<dynamic> AddPostMetadata(string queue, int startId, int endId, int limit)
    {
        var bsons = (await _grabberService.GetPostMetadataList(startId, limit)).ToList();

        if (bsons is null || bsons.Count == 0) return 0;

        bsons.RemoveAll(x => x["id"].AsInt32 > endId);

        var dataList = bsons.Select(x => new PostMetadata() { Data = x });
        var result = await _postMetadataRepository.AddRangeAsync(dataList);

        if (result.AddedIds.Count > 0)
            await _publishEndpoint.Publish(EventBuilders<T>.PostMetadataAddedIntegrationEventBuilder(result.AddedIds));

        if (result.ModifiedIds.Count > 0)
            await _publishEndpoint.Publish(EventBuilders<T>.PostMetadataModifiedIntegrationEventBuilder(result.ModifiedIds));

        if (result.UnchangedIds.Count > 0)
            await _publishEndpoint.Publish(EventBuilders<T>.PostMetadataUnchangedIntegrationEventBuilder(result.UnchangedIds));

        return new { Added = result.AddedIds.Count, Modified = result.ModifiedIds.Count, Unchanged = result.UnchangedIds.Count };
    }

    [Queue("{0}")]
    [AutomaticRetry(Attempts = 3)]
    public async Task<dynamic> ReplacePostMetadata(string queue, int startId, int endId, int limit)
    {
        var bsons = (await _grabberService.GetPostMetadataList(startId, limit)).ToList();

        if (bsons is null || bsons.Count == 0) return 0;

        bsons.RemoveAll(x => x["id"].AsInt32 > endId);

        var dataList = bsons.Select(x => new PostMetadata() { Data = x });
        var result = await _postMetadataRepository.ReplaceRangeAsync(dataList);

        if (result.AddedIds.Count > 0)
            await _publishEndpoint.Publish(EventBuilders<T>.PostMetadataAddedIntegrationEventBuilder(result.AddedIds));

        if (result.ModifiedIds.Count > 0)
            await _publishEndpoint.Publish(EventBuilders<T>.PostMetadataModifiedIntegrationEventBuilder(result.ModifiedIds));

        return new { Added = result.AddedIds.Count, Modified = result.ModifiedIds.Count };
    }
}
