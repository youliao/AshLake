using AshLake.BuildingBlocks.EventBus.Events;
using MongoDB.Driver;

namespace AshLake.Services.Archiver.Application.BackgroundJobs;

[Queue("postmetadata")]
public class PostMetadataJob<T> where T : ISouceSite
{
    private readonly IMetadataRepository<T, PostMetadata> _postMetadataRepository;
    private readonly IGrabberService<T> _grabberService;
    private readonly IEventBus _eventBus;

    public PostMetadataJob(IMetadataRepository<T, PostMetadata> postMetadataRepository, IGrabberService<T> grabberService, IEventBus eventBus)
    {
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task<dynamic> AddOrUpdatePostMetadata(int startId, int endId, int limit)
    {
        var bsons = (await _grabberService.GetPostMetadataList(startId, limit)).ToList();

        if (bsons is null || bsons.Count == 0) return 0;

        bsons.RemoveAll(x => x["id"].AsInt32 > endId);

        var dataList = bsons.Select(x => new PostMetadata() { Data = x });
        var result = await _postMetadataRepository.AddRangeAsync(dataList);

        if (result.AddedIds.Count > 0)
            await _eventBus.PublishAsync(PostMetadataAddedIntegrationEventBuilder(result.AddedIds));

        if (result.ModifiedIds.Count > 0)
            await _eventBus.PublishAsync(PostMetadataModifiedIntegrationEventBuilder(result.ModifiedIds));

        if (result.UnchangedIds.Count > 0)
            await _eventBus.PublishAsync(PostMetadataUnchangedIntegrationEventBuilder(result.UnchangedIds));

        return new { Added = result.AddedIds.Count, Modified = result.ModifiedIds.Count, Unchanged = result.UnchangedIds.Count };
    }
    public async Task<dynamic> ReplacePostMetadata(int startId, int endId, int limit)
    {
        var bsons = (await _grabberService.GetPostMetadataList(startId, limit)).ToList();

        if (bsons is null || bsons.Count == 0) return 0;

        bsons.RemoveAll(x => x["id"].AsInt32 > endId);

        var dataList = bsons.Select(x => new PostMetadata() { Data = x });
        var result = await _postMetadataRepository.ReplaceRangeAsync(dataList);

        if (result.AddedIds.Count > 0)
            await _eventBus.PublishAsync(PostMetadataAddedIntegrationEventBuilder(result.AddedIds));

        if (result.ModifiedIds.Count > 0)
            await _eventBus.PublishAsync(PostMetadataModifiedIntegrationEventBuilder(result.ModifiedIds));

        return new { Added = result.AddedIds.Count, Modified = result.ModifiedIds.Count };
    }

    private IntegrationEvent PostMetadataAddedIntegrationEventBuilder(IReadOnlyList<int> PostIds)
    {
        var souceSite = typeof(T).Name;

        IntegrationEvent integrationEvent = souceSite switch
        {
            nameof(Yande) => new YandePostMetadataAddedIntegrationEvent(PostIds),
            nameof(Danbooru) => new DanbooruPostMetadataAddedIntegrationEvent(PostIds),
            _ => throw new ArgumentOutOfRangeException(nameof(souceSite))
        };

        return integrationEvent;
    }

    private IntegrationEvent PostMetadataModifiedIntegrationEventBuilder(IReadOnlyList<int> PostIds)
    {
        var souceSite = typeof(T).Name;

        IntegrationEvent integrationEvent = souceSite switch
        {
            nameof(Yande) => new YandePostMetadataModifiedIntegrationEvent(PostIds),
            nameof(Danbooru) => new DanbooruPostMetadataModifiedIntegrationEvent(PostIds),
            _ => throw new ArgumentOutOfRangeException(nameof(souceSite))
        };

        return integrationEvent;
    }

    private IntegrationEvent PostMetadataUnchangedIntegrationEventBuilder(IReadOnlyList<int> PostIds)
    {
        var souceSite = typeof(T).Name;

        IntegrationEvent integrationEvent = souceSite switch
        {
            nameof(Yande) => new YandePostMetadataUnchangedIntegrationEvent(PostIds),
            nameof(Danbooru) => new DanbooruPostMetadataUnchangedIntegrationEvent(PostIds),
            _ => throw new ArgumentOutOfRangeException(nameof(souceSite))
        };

        return integrationEvent;
    }
}
