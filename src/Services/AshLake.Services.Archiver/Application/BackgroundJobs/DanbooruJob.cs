using AshLake.Services.Archiver.Domain.Services;
using MongoDB.Driver;

namespace AshLake.Services.Archiver.Application.BackgroundJobs;

[Queue("postmetadata")]
public class DanbooruJob
{
    private readonly IMetadataRepository<Danbooru, PostMetadata> _postMetadataRepository;
    private readonly IMetadataRepository<Danbooru, TagMetadata> _tagMetadataRepository;
    private readonly IDanbooruGrabberService _grabberService;
    private readonly IEventBus _eventBus;

    public DanbooruJob(IMetadataRepository<Danbooru, PostMetadata> postMetadataRepository, IMetadataRepository<Danbooru, TagMetadata> tagMetadataRepository, IDanbooruGrabberService grabberService, IEventBus eventBus)
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
            await _eventBus.PublishAsync(new PostMetadataAddedIntegrationEvent<Danbooru>(result.AddedIds));

        if (result.ModifiedIds.Count > 0)
            await _eventBus.PublishAsync(new PostMetadataModifiedIntegrationEvent<Danbooru>(result.ModifiedIds));

        if (result.UnchangedIds.Count > 0)
            await _eventBus.PublishAsync(new PostMetadataUnchangedIntegrationEvent<Danbooru>(result.UnchangedIds));

        return new { Added = result.AddedIds.Count, Modified = result.ModifiedIds.Count, Unchanged = result.UnchangedIds.Count };
    }

    public async Task<dynamic> AddOrUpdateTagMetadata(int type)
    {
        var bsons = (await _grabberService.GetTagMetadataList(type)).ToList();

        var dataList = bsons.Select(x => new TagMetadata() { Data = x }).Reverse();
        var result = await _tagMetadataRepository.AddRangeAsync(dataList);

        if (result.AddedIds.Count > 0 || result.ModifiedIds.Count > 0)
            await _eventBus.PublishAsync(new TagMetadataChangedIntegrationEvent<Danbooru>(type));

        return new { Added = result.AddedIds.Count, Modified = result.ModifiedIds.Count, Unchanged = result.UnchangedIds.Count };
    }
}
