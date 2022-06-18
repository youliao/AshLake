using AshLake.Contracts.Collector.Events;
using AshLake.Services.Collector.Domain.Repositories;

namespace AshLake.Services.Collector.Application.BackgroundJobs;

public class YandeJob
{
    private readonly IS3ObjectRepositoty<PostFile> _fileRepositoty;
    private readonly IYandeGrabberService _grabberService;
    private readonly IDownloadService _downloadService;
    private readonly IEventBus _eventBus;

    public YandeJob(IS3ObjectRepositoty<PostFile> fileRepositoty, IYandeGrabberService grabberService, IDownloadService downloadService, IEventBus eventBus)
    {
        _fileRepositoty = fileRepositoty ?? throw new ArgumentNullException(nameof(fileRepositoty));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
        _downloadService = downloadService ?? throw new ArgumentNullException(nameof(downloadService));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task<string> AddOrUpdateFile(int postId)
    {
        var link = await _grabberService.GetPostFileLink(postId);
        if (link is null) return EntityState.None.ToString();

        using var file = await _downloadService.DownloadFileTaskAsync(link.Url);

        var isExists = await _fileRepositoty.ExistsAsync(link.GetObjectId());

        var postFile = new PostFile(link.Md5, file)

        await _fileRepositoty.PutAsync(link);
        await _eventBus.PublishAsync(new PostFileChangedIntegrationEvent(link.ObjectKey));

        return isExists ? EntityState.Modified.ToString() : EntityState.Added.ToString();
    }

    public async Task<string> AddFile(int postId)
    {
        var objectKey = await _grabberService.GetPostObjectKey(postId);
        if (objectKey is null) return EntityState.None.ToString();

        var isExists = await _fileRepositoty.ExistsAsync(objectKey);
        if (isExists) return EntityState.Unchanged.ToString();

        var file = await _grabberService.GetPostFile(postId);
        if (file is null) return EntityState.None.ToString();

        await _fileRepositoty.PutAsync(file);
        await _eventBus.PublishAsync(new PostFileChangedIntegrationEvent(file.ObjectKey));
        return EntityState.Added.ToString();
    }
}
