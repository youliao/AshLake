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

        var processedIds = result.ProcessedRequests.Select(x => (x as ReplaceOneModel<PostMetadata>)!.Replacement.Id);

        var addedIds = result.Upserts.Select(x => x.Id.AsString);
        if (addedIds.Count() > 0)
            await _eventBus.PublishAsync(new PostMetadataAddedIntegrationEvent<Yande>(addedIds.ToList()));

        var modifiedIds = processedIds.Except(addedIds);
        if(modifiedIds.Count()>0)
            await _eventBus.PublishAsync(new PostMetadataModifiedIntegrationEvent<Yande>(modifiedIds.ToList()));

        return new { Added = addedIds.Count(), Modified = modifiedIds.Count() };
    }
}
