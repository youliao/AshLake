namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class YandeJob
{
    private const string PreviewStorageBindingName = "yande-preview-storage";
    private const string FileStorageBindingName = "yande-file-storage";
    private const string CreateBindingOperation = "create";

    private readonly DaprClient _daprClient;
    private readonly IYandeMetadataRepository<PostMetadata> _repository;
    private readonly IYandeGrabberService _grabberService;

    public YandeJob(DaprClient daprClient, IYandeMetadataRepository<PostMetadata> repository, IYandeGrabberService grabberService)
    {
        _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
    }

    [Queue("metadata")]
    public async Task<int> AddOrUpdatePostMetadata(int startId, int endId, int limit)
    {
        var metadataList = await _grabberService.GetPostMetadataList(startId, limit);

        if (metadataList is null || metadataList.Count == 0) return 0;

        metadataList.RemoveAll(x => x["id"].AsInt32 >= endId);

        foreach (var item in metadataList)
        {
            var postMetadata = new PostMetadata() { Data = item };
            var status = await _repository.AddOrUpdateAsync(postMetadata);
        }

        return metadataList.Count;
    }

    [Queue("preview")]
    public async Task AddOrUpdatePreview(int postId)
    {
        var stream = await _grabberService.GetPostPreview(postId);

        await _daprClient.InvokeBindingAsync(PreviewStorageBindingName,
                                             CreateBindingOperation,
                                             stream.ToBase64(),
                                             new Dictionary<string, string>() { { "key", $"{postId}.jpg" } });

    }

    [Queue("file")]
    public async Task AddOrUpdateFile(int postId)
    {
        (var stream,var fileExt) = await _grabberService.GetPostFile(postId);


        await _daprClient.InvokeBindingAsync(FileStorageBindingName,
                                             CreateBindingOperation,
                                             stream.ToBase64(),
                                             new Dictionary<string, string>() { { "key", $"{postId}.{fileExt}" } });

    }
}
