using AshLake.Contracts.Archiver.Events;

namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class YandeJob
{
    private readonly IMetadataRepository<Yande,PostMetadata> _postMetadataRepository;
    private readonly IPostImageRepositoty<PostFile> _fileRepositoty;
    private readonly IPostImageRepositoty<PostPreview> _previewRepositoty;
    private readonly IYandeGrabberService _grabberService;
    private readonly IEventBus _eventBus;

    public YandeJob(IMetadataRepository<Yande, PostMetadata> postMetadataRepository, IPostImageRepositoty<PostFile> fileRepositoty, IPostImageRepositoty<PostPreview> previewRepositoty, IYandeGrabberService grabberService, IEventBus eventBus)
    {
        _postMetadataRepository = postMetadataRepository ?? throw new ArgumentNullException(nameof(postMetadataRepository));
        _fileRepositoty = fileRepositoty ?? throw new ArgumentNullException(nameof(fileRepositoty));
        _previewRepositoty = previewRepositoty ?? throw new ArgumentNullException(nameof(previewRepositoty));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    [Queue("metadata")]
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
                case ArchiveStatus.Added:
                    await _eventBus.PublishAsync(new PostMetadataAddedIntegrationEvent<Yande>(postMetadata.Id));
                    break;
                case ArchiveStatus.Updated:
                    await _eventBus.PublishAsync(new PostMetadataUpdatedIntegrationEvent<Yande>(postMetadata.Id));
                    break;
                default:
                    break;
            };
        }

        return metadataList.Count;
    }

    [Queue("preview")]
    [MaximumConcurrentExecutions(1)]
    public async Task<string> AddOrUpdatePreview(int postId)
    {
        var preview = await _grabberService.GetPostPreview(postId);
        if (preview is null) return ArchiveStatus.None.ToString();

        var isExists = await _previewRepositoty.ExistsAsync(preview.ObjectKey);

        await _previewRepositoty.PutAsync(preview);
        return isExists ? ArchiveStatus.Updated.ToString() : ArchiveStatus.Added.ToString();
    }

    [Queue("preview")]
    [MaximumConcurrentExecutions(2)]
    public async Task<string> AddPreview(int postId)
    {
        var objectKey = await _grabberService.GetPostObjectKey(postId);
        if(objectKey is null) return ArchiveStatus.None.ToString();
        var isExists = await _previewRepositoty.ExistsAsync(objectKey);
        if (isExists) return ArchiveStatus.Untouched.ToString();

        var preview = await _grabberService.GetPostPreview(postId);
        if (preview is null) return ArchiveStatus.None.ToString();

        await _previewRepositoty.PutAsync(preview);
        return ArchiveStatus.Added.ToString();
    }

    [Queue("file")]
    [MaximumConcurrentExecutions(1)]
    public async Task<string> AddOrUpdateFile(int postId)
    {
        var file = await _grabberService.GetPostFile(postId);
        if (file is null) return ArchiveStatus.None.ToString();

        var isExists = await _fileRepositoty.ExistsAsync(file.ObjectKey);

        await _fileRepositoty.PutAsync(file);
        return isExists ? ArchiveStatus.Updated.ToString() : ArchiveStatus.Added.ToString();
    }

    [MaximumConcurrentExecutions(2)]
    [Queue("file")]
    public async Task<string> AddFile(int postId)
    {
        var objectKey = await _grabberService.GetPostObjectKey(postId);
        if (objectKey is null) return ArchiveStatus.None.ToString();
        var isExists = await _fileRepositoty.ExistsAsync(objectKey);
        if (isExists) return ArchiveStatus.Untouched.ToString();

        var file = await _grabberService.GetPostFile(postId);
        if (file is null) return ArchiveStatus.None.ToString();

        await _fileRepositoty.PutAsync(file);
        return ArchiveStatus.Added.ToString();
    }
}
