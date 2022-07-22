namespace AshLake.Services.Archiver.Application.IntegrationEvents;

public class DanbooruPostMetadataAddedEventConsumer : IConsumer<DanbooruPostMetadataAddedEvent>
{
    private readonly IPostRelationRepository _postRelationRepository;
    private readonly IMetadataRepository<Danbooru, PostMetadata> _postMetadataRepository;

    public DanbooruPostMetadataAddedEventConsumer(IPostRelationRepository postRelationRepository, IMetadataRepository<Danbooru, PostMetadata> postMetadataRepository)
    {
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
    }

    public async Task Consume(ConsumeContext<DanbooruPostMetadataAddedEvent> context)
    {
        var message = context.Message;

        var addedList = await _postMetadataRepository.FindAsync(x => message.PostIds.Contains(x.Id));

        var postRelations = addedList
            .Where(x => x.HasObjectKey<Danbooru>())
            .Select(x => new PostRelation
            {
                Id = x.GetObjectKey<Danbooru>()!,
                DanbooruId = x.Id
            });

        await _postRelationRepository.AddOrUpdatePostIdAsync<Danbooru>(postRelations);
    }
}
