using AshLake.BuildingBlocks.EventBus.Abstractions;
using AshLake.Contracts.Archiver.Events;
using AshLake.Contracts.Seedwork;
using AshLake.Services.Collector.Domain.Repositories;
using AshLake.Services.Collector.Integration.GrabberServices;
using Hangfire;

namespace AshLake.Services.Collector.Application.BackgroundJobs;

[Queue("yande")]
public class YandeJob
{
    private readonly IS3ObjectRepositoty<PostFile> _fileRepositoty;
    private readonly IS3ObjectRepositoty<PostPreview> _previewRepositoty;
    private readonly IYandeGrabberService _grabberService;

    public YandeJob(IS3ObjectRepositoty<PostFile> fileRepositoty, IS3ObjectRepositoty<PostPreview> previewRepositoty,
        IYandeGrabberService grabberService)
    {
        _fileRepositoty = fileRepositoty ?? throw new ArgumentNullException(nameof(fileRepositoty));
        _previewRepositoty = previewRepositoty ?? throw new ArgumentNullException(nameof(previewRepositoty));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
    }

    public async Task<string> AddOrUpdatePreview(int postId)
    {
        var preview = await _grabberService.GetPostPreview(postId);
        if (preview is null) return EntityState.None.ToString();

        var isExists = await _previewRepositoty.ExistsAsync(preview.ObjectKey);

        await _previewRepositoty.PutAsync(preview);
        return isExists ? EntityState.Modified.ToString() : EntityState.Added.ToString();
    }

    public async Task<string> AddPreview(int postId)
    {
        var objectKey = await _grabberService.GetPostObjectKey(postId);
        if(objectKey is null) return EntityState.None.ToString();
        var isExists = await _previewRepositoty.ExistsAsync(objectKey);
        if (isExists) return EntityState.Unchanged.ToString();

        var preview = await _grabberService.GetPostPreview(postId);
        if (preview is null) return EntityState.None.ToString();

        await _previewRepositoty.PutAsync(preview);
        return EntityState.Added.ToString();
    }

    public async Task<string> AddOrUpdateFile(int postId)
    {
        var file = await _grabberService.GetPostFile(postId);
        if (file is null) return EntityState.None.ToString();

        var isExists = await _fileRepositoty.ExistsAsync(file.ObjectKey);

        await _fileRepositoty.PutAsync(file);
        return isExists ? EntityState.Modified.ToString() : EntityState.Added.ToString();
    }

    public async Task<string> AddFile(int postId)
    {
        var objectKey = await _grabberService.GetPostObjectKey(postId);
        if (objectKey is null) return EntityState.None.ToString();
        var isExists = await _fileRepositoty.ExistsAsync(objectKey);
        if (isExists) return EntityState.Modified.ToString();

        var file = await _grabberService.GetPostFile(postId);
        if (file is null) return EntityState.None.ToString();

        await _fileRepositoty.PutAsync(file);
        return EntityState.Added.ToString();
    }
}
