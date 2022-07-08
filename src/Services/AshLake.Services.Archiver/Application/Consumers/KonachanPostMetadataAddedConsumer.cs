namespace AshLake.Services.Archiver.Application.Consumers;

public class KonachanPostMetadataAddedConsumer : IConsumer<KonachanPostMetadataAddedEvent>
{
    private readonly IPostRelationRepository _postRelationRepository;
    private readonly IMetadataRepository<Konachan, PostMetadata> _postMetadataRepository;

    public KonachanPostMetadataAddedConsumer(IPostRelationRepository postRelationRepository, IMetadataRepository<Konachan, PostMetadata> postMetadataRepository)
    {
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
    }

    public async Task Consume(ConsumeContext<KonachanPostMetadataAddedEvent> context)
    {
        var message = context.Message;

        var addedList = await _postMetadataRepository.FindAsync(x => message.PostIds.Contains(x.Id));
        var dic = addedList.ToDictionary(k => k.Data.GetValue(KonachanPostMetadataKeys.md5).AsString, v => v.Id);

        await _postRelationRepository.AddOrUpdateRangeAsync<Konachan>(dic);
    }
}
