using AshLake.Services.YandeStore.Infrastructure.Services;

namespace AshLake.Services.YandeStore.Application.BackgroundJobs;

[Queue("post")]
public class PostJob
{
    private readonly IMediator _mediator;
    private readonly IYandeArchiverService _archiverService;

    public PostJob(IMediator mediator, IYandeArchiverService archiverService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _archiverService = archiverService ?? throw new ArgumentNullException(nameof(archiverService));
    }

    public async Task<int> AddOrUpdatePost(int postId)
    {
        var metadata = await _archiverService.GetPostMetadata(postId);
        var command = metadata.Adapt<AddOrUpdatePostCommand>();

        return await _mediator.Send(command);
    }

    public async Task<int> BulkAddPosts(IEnumerable<int> postIds)
    {
        var bsons = await _archiverService.GetPostMetadataByRange(postIds.Min(), postIds.Max());
        var commands = new List<AddOrUpdatePostCommand>();

        foreach (var item in bsons)
        {
            if (!postIds.Contains(item.GetValue(YanderePostMetadataKeys.id).AsInt32))
                continue;

            var command = item.Adapt<AddOrUpdatePostCommand>();
            commands.Add(command);
        }

        return await _mediator.Send(new BulkAddPostsCommand(commands));
    }

    public async Task<int> BulkUpdatePosts(IEnumerable<int> postIds)
    {
        var bsons = await _archiverService.GetPostMetadataByRange(postIds.Min(), postIds.Max());
        var commands = new List<AddOrUpdatePostCommand>();

        foreach (var item in bsons)
        {
            if (!postIds.Contains(item.GetValue(YanderePostMetadataKeys.id).AsInt32))
                continue;
            var command = item.Adapt<AddOrUpdatePostCommand>();
            commands.Add(command);
        }

        return await _mediator.Send(new BulkUpdatePostsCommand(commands));
    }
}
