namespace AshLake.Services.Archiver.Application.EventConsumers;

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

        var postRelations = addedList
            .Where(x => x.HasObjectKey<Yandere>())
            .Select(x => new PostRelation
            {
                Id = x.GetObjectKey<Yandere>()!,
                YandereId = x.Id
            });

        await _postRelationRepository.AddOrUpdateRangeAsync<Yandere>(postRelations);
    }
}
