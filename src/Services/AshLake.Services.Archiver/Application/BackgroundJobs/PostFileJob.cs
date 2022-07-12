namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class PostFileJob
{
    private readonly IPostRelationRepository _postRelationRepository;
    private readonly IMediator _mediator;

    public PostFileJob(IPostRelationRepository postRelationRepository, IMediator mediator)
    {
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Queue("common")]
    [AutomaticRetry(Attempts = 3)]
    public async Task InitializePostFileStatus(int limit)
    {
        var command = new InitializePostFileStatusCommand(limit);
        await _mediator.Send(command);
    }

    [Queue("common")]
    [AutomaticRetry(Attempts = 3)]
    public async Task SyncPostFileStatus(int limit)
    {
        var command = new SyncPostFileStatusCommand(limit);
        await _mediator.Send(command);
    }

    [Queue("common")]
    [AutomaticRetry(Attempts = 3)]
    public async Task<int> DownloadManyPostFiles(int limit,int max = 1000)
    {
        var downloadingQueueLength = await _postRelationRepository.CountAsync(x => x.FileStatus == PostFileStatus.Downloading);

        if (downloadingQueueLength > max) return 0;

        var postRelations = await _postRelationRepository.FindAsync(x => x.FileStatus == PostFileStatus.None, limit);

        if(postRelations.Count() == 0) return 0;

        var command = new CreateManyPostFileDownloadTasksCommnad(postRelations.Select(x=>x.Id));
        await _mediator.Send(command);

        return postRelations.Count();

    }
}
