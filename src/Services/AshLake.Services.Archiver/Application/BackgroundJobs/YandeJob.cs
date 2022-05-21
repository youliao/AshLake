namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class YandeJob
{
    private const string PreviewStorageBindingName = "yande-preview-storage";
    private const string FileStorageBindingName = "yande-file-storage";
    private const string CreateBindingOperation = "create";

    private readonly DaprClient _daprClient;
    private readonly IYandeMetadataRepository<PostMetadata> _postMetadataRepository;
    private readonly IYandePostFileRepositoty _postFileRepositoty;
    private readonly IYandeGrabberService _grabberService;

    public YandeJob(DaprClient daprClient, IYandeMetadataRepository<PostMetadata> postMetadataRepository, IYandePostFileRepositoty postFileRepositoty, IYandeGrabberService grabberService)
    {
        _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
        _postFileRepositoty = postFileRepositoty ?? throw new ArgumentNullException(nameof(postFileRepositoty));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
    }

    [Queue("metadata")]
    public async Task<int> AddOrUpdatePostMetadata(int startId, int endId, int limit)
    {
        var metadataList = (await _grabberService.GetPostMetadataList(startId, limit)).ToList();

        if (metadataList is null || metadataList.Count == 0) return 0;

        metadataList.RemoveAll(x => x["id"].AsInt32 >= endId);

        foreach (var item in metadataList)
        {
            var postMetadata = new PostMetadata() { Data = item };
            var status = await _postMetadataRepository.AddOrUpdateAsync(postMetadata);
        }

        return metadataList.Count;
    }

    [Queue("preview")]
    public async Task AddOrUpdatePreview(int postId)
    {
        var stream = await _grabberService.GetPostPreview(postId);

        await _daprClient.InvokeBindingAsync(PreviewStorageBindingName,
                                             CreateBindingOperation,
                                             stream,
                                             new Dictionary<string, string>() { { "key", $"{postId}.jpg" } });

    }

    [Queue("file")]
    public async Task<string> AddOrUpdateFile(int postId)
    {
        (var stream,var fileExt) = await _grabberService.GetPostFile(postId);
        var objectKey = $"{postId}.{fileExt}";

        return await _postFileRepositoty.AddOrUpdateAsync(objectKey, stream);
    }
}
