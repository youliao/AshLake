using AshLake.Contracts.Collector.Events;
using AshLake.Services.Collector.Domain.Repositories;

namespace AshLake.Services.Collector.Application.BackgroundJobs;

public class YandeJob
{
    private readonly IS3ObjectRepositoty<PostFile> _fileRepositoty;
    private readonly IYandeGrabberService _grabberService;
    private readonly IEventBus _eventBus;

    public YandeJob(IS3ObjectRepositoty<PostFile> fileRepositoty, IYandeGrabberService grabberService, IEventBus eventBus)
    {
        _fileRepositoty = fileRepositoty ?? throw new ArgumentNullException(nameof(fileRepositoty));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task<string> AddOrUpdateFile(int postId)
    {
        var file = await _grabberService.GetPostFile(postId);
        if (file is null) return EntityState.None.ToString();

        var isExists = await _fileRepositoty.ExistsAsync(file.ObjectKey);

        await _fileRepositoty.PutAsync(file);
        await _eventBus.PublishAsync(new PostFileChangedIntegrationEvent(file.ObjectKey));

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
