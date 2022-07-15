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
        var command = new InitializePostRelationCommand(limit);
        await _mediator.Send(command);
    }

    [Queue("common")]
    [AutomaticRetry(Attempts = 3)]
    public async Task RecheckDownloadingStatus(int limit)
    {
        var command = new RecheckDownloadingStatusCommand(limit);
        await _mediator.Send(command);
    }

    [Queue("common")]
    [AutomaticRetry(Attempts = 3)]
    public async Task DownloadManyPostFiles(int limit)
    {
        var command = new CreateManyPostFileDownloadTasksCommnad(limit);
        await _mediator.Send(command);
    }
}
