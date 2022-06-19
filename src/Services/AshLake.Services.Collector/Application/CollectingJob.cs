using AshLake.Contracts.Collector.Events;

namespace AshLake.Services.Collector.Application;

public class CollectingJob<T> where T : ISouceSite
{
    private readonly IGrabberService<T> _grabberService;
    private readonly IDownloader _downloader;
    private readonly IS3ObjectRepositoty<PostFile> _fileRepositoty;
    private readonly IEventBus _eventBus;

    public CollectingJob(IGrabberService<T> grabberService, IDownloader downloader, IS3ObjectRepositoty<PostFile> fileRepositoty, IEventBus eventBus)
    {
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
        _downloader = downloader ?? throw new ArgumentNullException(nameof(downloader));
        _fileRepositoty = fileRepositoty ?? throw new ArgumentNullException(nameof(fileRepositoty));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task<string> AddOrUpdateFile(int postId)
    {
        var link = await _grabberService.GetPostFileLink(postId);
        if (link is null) return EntityState.None.ToString();

        var data = await _downloader.DownloadFileTaskAsync(link.Url);

        var isExists = await _fileRepositoty.ExistsAsync(link.ObjectKey);

        var postFile = new PostFile(link.ObjectKey, data);

        await _fileRepositoty.PutAsync(postFile);
        await _eventBus.PublishAsync(new PostFileChangedIntegrationEvent(link.ObjectKey));

        return isExists ? EntityState.Modified.ToString() : EntityState.Added.ToString();
    }

    public async Task<string> AddFile(int postId)
    {
        var link = await _grabberService.GetPostFileLink(postId);
        if (link is null) return EntityState.None.ToString();

        var isExists = await _fileRepositoty.ExistsAsync(link.ObjectKey);
        if (isExists) return EntityState.Unchanged.ToString();

        var data = await _downloader.DownloadFileTaskAsync(link.Url);

        var postFile = new PostFile(link.ObjectKey, data);

        await _fileRepositoty.PutAsync(postFile);
        await _eventBus.PublishAsync(new PostFileChangedIntegrationEvent(link.ObjectKey));
        return EntityState.Added.ToString();
    }
}
