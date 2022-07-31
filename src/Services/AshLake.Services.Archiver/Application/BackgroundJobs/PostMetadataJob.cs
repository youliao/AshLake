using MongoDB.Driver;

namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class PostMetadataJob<T> where T : Booru
{
    private readonly IMetadataRepository<T, PostMetadata> _postMetadataRepository;
    private readonly IBooruApiService<T> _grabberService;
    private readonly DaprClient _dapr;

    public PostMetadataJob(IMetadataRepository<T, PostMetadata> postMetadataRepository, IBooruApiService<T> grabberService, DaprClient dapr)
    {
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
        _dapr = dapr ?? throw new ArgumentNullException(nameof(dapr));
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
        {
            var @event = EventBuilders<T>.PostMetadataAddedIntegrationEventBuilder(result.AddedIds);
            await _dapr.PublishEventAsync(DaprInfo.DAPR_PUBSUB_NAME, @event.GetType().Name, @event);
        }

        if (result.ModifiedIds.Count > 0)
        {
            var @event = EventBuilders<T>.PostMetadataModifiedIntegrationEventBuilder(result.ModifiedIds);
            await _dapr.PublishEventAsync(DaprInfo.DAPR_PUBSUB_NAME, @event.GetType().Name, @event);
        }

        if (result.UnchangedIds.Count > 0)
        {
            var @event = EventBuilders<T>.PostMetadataUnchangedIntegrationEventBuilder(result.UnchangedIds);
            await _dapr.PublishEventAsync(DaprInfo.DAPR_PUBSUB_NAME, @event.GetType().Name, @event);
        }


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
        {
            var @event = EventBuilders<T>.PostMetadataAddedIntegrationEventBuilder(result.AddedIds);
            await _dapr.PublishEventAsync(DaprInfo.DAPR_PUBSUB_NAME, @event.GetType().Name, @event);
        }

        if (result.ModifiedIds.Count > 0)
        {
            var @event = EventBuilders<T>.PostMetadataModifiedIntegrationEventBuilder(result.ModifiedIds);
            await _dapr.PublishEventAsync(DaprInfo.DAPR_PUBSUB_NAME, @event.GetType().Name, @event);
        }

        return new { Added = result.AddedIds.Count, Modified = result.ModifiedIds.Count };
    }
}
