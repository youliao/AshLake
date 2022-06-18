using AshLake.Contracts.Collector.Events;
using AshLake.Services.Collector.Domain.Repositories;

namespace AshLake.Services.Collector.Application.BackgroundJobs;

public class YandeJob
{
    private readonly IS3ObjectRepositoty<PostFile> _fileRepositoty;
    private readonly IGrabberService<Yande> _grabberService;
    private readonly IDownloadService _downloadService;
    private readonly IEventBus _eventBus;

    public YandeJob(IS3ObjectRepositoty<PostFile> fileRepositoty, IGrabberService<Yande> grabberService, IDownloadService downloadService, IEventBus eventBus)
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

        byte[] bytes = new byte[data.Length];
        data.Read(bytes, 0, bytes.Length);
        data.Seek(0, SeekOrigin.Begin);
        var postFile = new PostFile(link.Md5, bytes, objectKey);

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
        if (data.Length == 0) throw new ArgumentNullException();
        MemoryStream ms = new MemoryStream();
        data.CopyTo(ms);

        var postFile = new PostFile(link.Md5, ms.ToArray(), objectKey);

        await _fileRepositoty.PutAsync(postFile);
        await _eventBus.PublishAsync(new PostFileChangedIntegrationEvent(objectKey));

        return EntityState.Added.ToString();
    }
}
