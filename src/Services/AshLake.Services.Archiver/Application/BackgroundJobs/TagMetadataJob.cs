using MongoDB.Driver;

namespace AshLake.Services.Archiver.Application.BackgroundJobs;

[Queue("tagmetadata")]
public class TagMetadataJob<T> where T : IBooru
{
    private readonly IMetadataRepository<T, TagMetadata> _tagMetadataRepository;
    private readonly IBooruApiService<T> _grabberService;
    private readonly IPublishEndpoint _publishEndpoint;

    public TagMetadataJob(IMetadataRepository<T, TagMetadata> tagMetadataRepository, IBooruApiService<T> grabberService, IPublishEndpoint publishEndpoint)
    {
        _tagMetadataRepository = tagMetadataRepository ?? throw new ArgumentNullException(nameof(tagMetadataRepository));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<dynamic> AddOrUpdateTagMetadata(int type)
    {
        var bsons = (await _grabberService.GetTagMetadataList(type)).ToList();

        var dataList = bsons.Select(x => new TagMetadata() { Data = x }).Reverse();
        var result = await _tagMetadataRepository.AddRangeAsync(dataList);

        if (result.AddedIds.Count > 0 || result.ModifiedIds.Count > 0)
            await _publishEndpoint.Publish(new DanbooruTagMetadataChangedEvent(type));

        return new { Added = result.AddedIds.Count, Modified = result.ModifiedIds.Count, Unchanged = result.UnchangedIds.Count };
    }
}
