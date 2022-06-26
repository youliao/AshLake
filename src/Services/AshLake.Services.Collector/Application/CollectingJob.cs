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

    [Queue("{0}")]
    public async Task<dynamic> AddFiles(string queue, IReadOnlyList<int> postIds)
    {
        int added = 0;
        int unchanged = 0;
        int none = 0;

        await Parallel.ForEachAsync(postIds, new ParallelOptions { MaxDegreeOfParallelism = 5 }, async (postId, _) =>
        {
            var objectKey = await _archiverService.GetPostObjectKey(postId);

            if (objectKey is null)
            {
                none++;
                return;
            }

            var exists = await _fileRepositoty.ExistsAsync(objectKey!);
            if (exists)
            {
                unchanged++;
                return;
            }

            var fileUrl = await _grabberService.GetPostFileUrl(postId);
            if (fileUrl is null)
            {
                none++;
                return;
            }

            var data = await _downloader.DownloadFileAsync(fileUrl!);

            var postFile = new PostFile(objectKey!, data);

            await _fileRepositoty.PutAsync(postFile);
            await _eventBus.PublishAsync(new PostFileChangedIntegrationEvent(objectKey!));

            added++;
        });

        return new { Added = added, Unchanged = unchanged, None = none };
    }
}
