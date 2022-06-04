using MongoDB.Driver;

namespace AshLake.Services.Archiver.Application.BackgroundJobs;

[Queue("yande")]
public class YandeJob
{
    private readonly IMetadataRepository<Yande,PostMetadata> _postMetadataRepository;
    private readonly IYandeGrabberService _grabberService;
    private readonly IEventBus _eventBus;

    public YandeJob(IMetadataRepository<Yande, PostMetadata> postMetadataRepository,IYandeGrabberService grabberService, IEventBus eventBus)
    {
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }


    public async Task<object> AddOrUpdatePostMetadata(int startId, int endId, int limit)
    {
        var bsons = (await _grabberService.GetPostMetadataList(startId, limit)).ToList();

        if (bsons is null || bsons.Count == 0) return 0;

        bsons.RemoveAll(x => x["id"].AsInt32 > endId);

        var postMetadatas = bsons.Select(x => new PostMetadata() { Data = x });
        var result = await _postMetadataRepository.AddRangeAsync(postMetadatas);

        if (result.AddedIds.Count > 0)
            await _eventBus.PublishAsync(new PostMetadataAddedIntegrationEvent<Yande>(result.AddedIds));

        if (result.ModifiedIds.Count > 0)
            await _eventBus.PublishAsync(new PostMetadataModifiedIntegrationEvent<Yande>(result.ModifiedIds));

        if (result.UnchangedIds.Count > 0)
            await _eventBus.PublishAsync(new PostMetadataUnchangedIntegrationEvent<Yande>(result.UnchangedIds));

        return new { Added = result.AddedIds.Count, Modified = result.ModifiedIds.Count, Unchanged = result.UnchangedIds.Count };
    }
}
