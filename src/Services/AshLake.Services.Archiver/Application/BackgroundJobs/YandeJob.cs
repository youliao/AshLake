namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class YandeJob
{
    private const string _qName = "yande";

    private readonly IMetadataRepository<Yande,PostMetadata> _postMetadataRepository;
    private readonly IYandeGrabberService _grabberService;
    private readonly IEventBus _eventBus;

    public YandeJob(IMetadataRepository<Yande, PostMetadata> postMetadataRepository,IYandeGrabberService grabberService, IEventBus eventBus)
    {
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    [Queue(_qName)]
    public async Task<int> AddOrUpdatePostMetadata(int startId, int endId, int limit)
    {
        var metadataList = (await _grabberService.GetPostMetadataList(startId, limit)).ToList();

        if (metadataList is null || metadataList.Count == 0) return 0;

        metadataList.RemoveAll(x => x["id"].AsInt32 > endId);

        foreach (var item in metadataList)
        {
            var postMetadata = new PostMetadata() { Data = item };
            var status = await _postMetadataRepository.AddOrUpdateAsync(postMetadata);

            switch (status)
            {
                case EntityState.Added:
                    await _eventBus.PublishAsync(new PostMetadataAddedIntegrationEvent<Yande>(postMetadata.Id));
                    break;
                case EntityState.Modified:
                    await _eventBus.PublishAsync(new PostMetadataUpdatedIntegrationEvent<Yande>(postMetadata.Id));
                    break;
                default:
                    break;
            };
        }

        return metadataList.Count;
    }
}
