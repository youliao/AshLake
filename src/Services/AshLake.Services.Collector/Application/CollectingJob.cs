using AshLake.Contracts.Collector.Events;

namespace AshLake.Services.Collector.Application;

public class CollectingJob<T> where T : ISouceSite
{
    private readonly IGrabberService<T> _grabberService;
    private readonly IArchiverService<T> _archiverService;
    private readonly IDownloader _downloader;
    private readonly IS3ObjectRepositoty<PostFile> _fileRepositoty;
    private readonly IEventBus _eventBus;

    public CollectingJob(IGrabberService<T> grabberService, IArchiverService<T> archiverService, IDownloader downloader, IS3ObjectRepositoty<PostFile> fileRepositoty, IEventBus eventBus)
    {
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
        _archiverService = archiverService ?? throw new ArgumentNullException(nameof(archiverService));
        _downloader = downloader ?? throw new ArgumentNullException(nameof(downloader));
        _fileRepositoty = fileRepositoty ?? throw new ArgumentNullException(nameof(fileRepositoty));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task<string> AddOrUpdateFile(int postId)
    {
        var objectKey = await _archiverService.GetPostObjectKey(postId);
        if(objectKey is null) return EntityState.None.ToString();

        var stat = await _fileRepositoty.StatObjectAsync(objectKey);
        var fileMd5 = Path.GetFileNameWithoutExtension(objectKey);
        if (stat is not null && stat.ETag == fileMd5) return EntityState.Unchanged.ToString();

        var fileUrl = await _grabberService.GetPostFileUrl(postId);
        if (fileUrl is null && stat is null) return EntityState.None.ToString();
        if (fileUrl is null && stat is not null) return EntityState.Unchanged.ToString();

        var data = await _downloader.DownloadFileAsync(fileUrl!);

        var postFile = new PostFile(objectKey, data);

        await _fileRepositoty.PutAsync(postFile);
        await _eventBus.PublishAsync(new PostFileChangedIntegrationEvent(objectKey));

        return stat is null ? EntityState.Added.ToString() : EntityState.Modified.ToString();
    }
}
