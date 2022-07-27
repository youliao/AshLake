namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class PostFileJobs
{
    private readonly IMediator _mediator;

    public PostFileJobs(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Queue("common")]
    [AutomaticRetry(Attempts = 3)]
    public async Task InitializePostRelationJob(InitializePostRelation command)
    {
        await _mediator.Send(command);
    }

    [Queue("common")]
    [AutomaticRetry(Attempts = 3)]
    public async Task<int> DownloadManyPostFilesJob(CreateManyPostFileDownloadTasks command)
    {
        var result = await _mediator.SendRequest(command);

        return result.TaskIds.Count();
    }
}
