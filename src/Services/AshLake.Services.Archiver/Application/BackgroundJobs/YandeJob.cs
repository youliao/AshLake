namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class YandeJob
{
    private readonly IMetadataRepository<Yande,PostMetadata> _postMetadataRepository;
    private readonly IPostImageRepositoty<PostFile> _postFileRepositoty;
    private readonly IPostImageRepositoty<PostPreview> _postPreviewRepositoty;
    private readonly IYandeGrabberService _grabberService;

    public YandeJob(IMetadataRepository<Yande, PostMetadata> postMetadataRepository, IPostImageRepositoty<PostFile> postFileRepositoty, IPostImageRepositoty<PostPreview> postPreviewRepositoty, IYandeGrabberService grabberService)
    {
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
        _postFileRepositoty = postFileRepositoty ?? throw new ArgumentNullException(nameof(postFileRepositoty));
        _postPreviewRepositoty = postPreviewRepositoty ?? throw new ArgumentNullException(nameof(postPreviewRepositoty));
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
        var data = await _grabberService.GetPostPreview(postId);

        var preview = new PostPreview($"{postId}", data);

        await _postPreviewRepositoty.PutAsync(preview);

    }

    [Queue("file")]
    public async Task AddOrUpdateFile(int postId)
    {
        (var data,var fileExt) = await _grabberService.GetPostFile(postId);

        var file = new PostFile($"{postId}",ImageType.JPG ,data);

        await _postFileRepositoty.PutAsync(file);
    }
}
