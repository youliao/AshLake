using AshLake.Services.YandeStore.Application.Services;

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

    public async Task<int> AddPost(string postId)
    {
        var metadata = await _archiverService.GetPostMetadata(int.Parse(postId));
        var command = metadata.Adapt<AddPostCommand>();

        return await _mediator.Send(command);
    }

    public async Task<int> UpdatePost(string postId)
    {
        var metadata = await _archiverService.GetPostMetadata(int.Parse(postId));
        var command = metadata.Adapt<UpdatePostCommand>();

        return await _mediator.Send(command);
    }
}
