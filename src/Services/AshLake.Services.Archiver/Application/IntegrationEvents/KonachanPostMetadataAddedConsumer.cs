namespace AshLake.Services.Archiver.Application.IntegrationEvents;

public class KonachanPostMetadataAddedConsumer : IConsumer<KonachanPostMetadataAdded>
{
    private readonly IPostRelationRepository _postRelationRepository;
    private readonly IMetadataRepository<Konachan, PostMetadata> _postMetadataRepository;

    public KonachanPostMetadataAddedConsumer(IPostRelationRepository postRelationRepository, IMetadataRepository<Konachan, PostMetadata> postMetadataRepository)
    {
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
    }

    public async Task Consume(ConsumeContext<KonachanPostMetadataAdded> context)
    {
        var message = context.Message;

        var addedList = await _postMetadataRepository.FindAsync(x => message.PostIds.Contains(x.Id));

        var postRelations = addedList
            .Where(x => x.HasObjectKey<Konachan>())
            .Select(x => new PostRelation
            {
                Id = x.GetObjectKey<Konachan>()!,
                KonachanId = x.Id
            });

        await _postRelationRepository.AddOrUpdatePostIdAsync<Konachan>(postRelations);
    }
}
