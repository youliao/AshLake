namespace AshLake.Services.Archiver.Application.Consumers;

public class YanderePostMetadataAddedEventConsumer : IConsumer<YanderePostMetadataAddedEvent>
{
    private readonly IPostRelationRepository _postRelationRepository;
    private readonly IMetadataRepository<Yandere, PostMetadata> _postMetadataRepository;

    public YanderePostMetadataAddedEventConsumer(IPostRelationRepository postRelationRepository, IMetadataRepository<Yandere, PostMetadata> postMetadataRepository)
    {
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
    }

    public async Task Consume(ConsumeContext<YanderePostMetadataAddedEvent> context)
    {
        var message = context.Message;

        var addedList = await _postMetadataRepository.FindAsync(x => message.PostIds.Contains(x.Id));
        var dic = addedList.ToDictionary(k => k.Data.GetValue(YanderePostMetadataKeys.md5).AsString, v => v.Id);

        await _postRelationRepository.AddOrUpdateRangeAsync<Yandere>(dic);
    }
}
