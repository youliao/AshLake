namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class PostFileJob
{
    private readonly IMediator _mediator;

    public PostFileJob(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Queue("common")]
    [AutomaticRetry(Attempts = 3)]
    public async Task InitializePostRelation(int limit)
    {
        var command = new InitializePostRelation(limit);
        await _mediator.Send(command);
    }

    [Queue("common")]
    [AutomaticRetry(Attempts = 3)]
    public async Task<dynamic> RecheckDownloadingStatus(int limit)
    {
        var command = new RecheckDownloadingTasks(limit);
        var result = await _mediator.SendRequest(command);

        return result;
    }

    [Queue("common")]
    [AutomaticRetry(Attempts = 3)]
    public async Task DownloadManyPostFiles(int limit)
    {
        var command = new CreateManyPostFileDownloadTasks(limit);
        await _mediator.Send(command);
    }
}
