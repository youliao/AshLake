using MongoDB.Driver;

namespace AshLake.Services.Archiver.Application.BackgroundJobs;

[Queue("tagmetadata")]
public class TagMetadataJob<T> where T : IBooru
{
    private readonly IMetadataRepository<T, TagMetadata> _tagMetadataRepository;
    private readonly IGrabberService<T> _grabberService;
    private readonly IEventBus _eventBus;

    public TagMetadataJob(IMetadataRepository<T, TagMetadata> tagMetadataRepository, IGrabberService<T> grabberService, IEventBus eventBus)
    {
        _tagMetadataRepository = tagMetadataRepository ?? throw new ArgumentNullException(nameof(tagMetadataRepository));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task<dynamic> AddOrUpdateTagMetadata(int type)
    {
        var bsons = (await _grabberService.GetTagMetadataList(type)).ToList();

        var dataList = bsons.Select(x => new TagMetadata() { Data = x }).Reverse();
        var result = await _tagMetadataRepository.AddRangeAsync(dataList);

        if (result.AddedIds.Count > 0 || result.ModifiedIds.Count > 0)
            await _eventBus.PublishAsync(new DanbooruTagMetadataChangedIntegrationEvent(type));

        return new { Added = result.AddedIds.Count, Modified = result.ModifiedIds.Count, Unchanged = result.UnchangedIds.Count };
    }
}
