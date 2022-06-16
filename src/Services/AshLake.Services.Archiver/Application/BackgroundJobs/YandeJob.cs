using AshLake.Services.Archiver.Domain.Services;
using MongoDB.Driver;

namespace AshLake.Services.Archiver.Application.BackgroundJobs;

[Queue("postmetadata")]
public class YandeJob
{
    private readonly IMetadataRepository<Yande, PostMetadata> _postMetadataRepository;
    private readonly IMetadataRepository<Yande, TagMetadata> _tagMetadataRepository;
    private readonly IYandeGrabberService _grabberService;
    private readonly IEventBus _eventBus;

    public YandeJob(IMetadataRepository<Yande, PostMetadata> postMetadataRepository, IMetadataRepository<Yande, TagMetadata> tagMetadataRepository, IYandeGrabberService grabberService, IEventBus eventBus)
    {
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
        _tagMetadataRepository = tagMetadataRepository ?? throw new ArgumentNullException(nameof(tagMetadataRepository));
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
            await _eventBus.PublishAsync(new YandePostMetadataAddedIntegrationEvent(result.AddedIds));

        if (result.ModifiedIds.Count > 0)
            await _eventBus.PublishAsync(new YandePostMetadataAddedIntegrationEvent(result.ModifiedIds));

        if (result.UnchangedIds.Count > 0)
            await _eventBus.PublishAsync(new YandePostMetadataAddedIntegrationEvent(result.UnchangedIds));

        return new { Added = result.AddedIds.Count, Modified = result.ModifiedIds.Count, Unchanged = result.UnchangedIds.Count };
    }

    public async Task<dynamic> AddOrUpdateTagMetadata(int type)
    {
        var bsons = (await _grabberService.GetTagMetadataList(type)).ToList();

        var dataList = bsons.Select(x => new TagMetadata() { Data = x }).Reverse();
        var result = await _tagMetadataRepository.AddRangeAsync(dataList);

        if (result.AddedIds.Count > 0 || result.ModifiedIds.Count > 0)
            await _eventBus.PublishAsync(new YandeTagMetadataChangedIntegrationEvent(type));

        return new { Added = result.AddedIds.Count, Modified = result.ModifiedIds.Count, Unchanged = result.UnchangedIds.Count };
    }
}
