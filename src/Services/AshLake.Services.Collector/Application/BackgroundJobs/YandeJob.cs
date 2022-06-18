using AshLake.Contracts.Collector.Events;
using AshLake.Services.Collector.Domain.Repositories;

namespace AshLake.Services.Collector.Application.BackgroundJobs;

public class YandeJob
{
    private readonly IS3ObjectRepositoty<PostFile> _fileRepositoty;
    private readonly IGrabberService _grabberService;
    private readonly IDownloadService _downloadService;
    private readonly IEventBus _eventBus;

    public YandeJob(IS3ObjectRepositoty<PostFile> fileRepositoty, IGrabberService grabberService, IDownloadService downloadService, IEventBus eventBus)
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

        using var data = await _downloadService.DownloadFileTaskAsync(link.Url);

        var objectKey = link.GetObjectKey();

        var isExists = await _fileRepositoty.ExistsAsync(objectKey);

        var postFile = new PostFile(link.Md5, data, objectKey);

        await _fileRepositoty.PutAsync(postFile);
        await _eventBus.PublishAsync(new PostFileChangedIntegrationEvent(objectKey));

        return isExists ? EntityState.Modified.ToString() : EntityState.Added.ToString();
    }

    public async Task<string> AddFile(int postId)
    {
        var link = await _grabberService.GetPostFileLink(postId);
        if (link is null) return EntityState.None.ToString();

        var objectKey = link.GetObjectKey();

        var isExists = await _fileRepositoty.ExistsAsync(objectKey);
        if (isExists) return EntityState.Unchanged.ToString();

        using var data = await _downloadService.DownloadFileTaskAsync(link.Url);
        var postFile = new PostFile(link.Md5, data, objectKey);

        await _fileRepositoty.PutAsync(postFile);
        await _eventBus.PublishAsync(new PostFileChangedIntegrationEvent(objectKey));
        return EntityState.Added.ToString();
    }
}
