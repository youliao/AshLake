namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class YandeJob
{
    private readonly IYandeMetadataRepository<PostMetadata> _postMetadataRepository;
    private readonly IYandeFileRepositoty _fileRepositoty;
    private readonly IYandePreviewRepositoty _previewRepositoty;
    private readonly IYandeGrabberService _grabberService;

    public YandeJob(IYandeMetadataRepository<PostMetadata> postMetadataRepository, IYandeFileRepositoty fileRepositoty, IYandePreviewRepositoty previewRepositoty, IYandeGrabberService grabberService)
    {
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
        _fileRepositoty = fileRepositoty ?? throw new ArgumentNullException(nameof(fileRepositoty));
        _previewRepositoty = previewRepositoty ?? throw new ArgumentNullException(nameof(previewRepositoty));
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
    public async Task<string> AddOrUpdatePreview(int postId)
    {
        var stream = await _grabberService.GetPostPreview(postId);

        var objectKey = $"{postId}.jpg";
        return await _previewRepositoty.AddOrUpdateAsync(objectKey, stream);

    }

    [Queue("file")]
    public async Task<string> AddOrUpdateFile(int postId)
    {
        (var stream,var fileExt) = await _grabberService.GetPostFile(postId);
        var objectKey = $"{postId}.{fileExt}";

        return await _fileRepositoty.AddOrUpdateAsync(objectKey, stream);
    }
}
