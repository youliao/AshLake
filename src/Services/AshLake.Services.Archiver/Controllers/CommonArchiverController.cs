namespace AshLake.Services.Archiver.Controllers;

public class CommonArchiverController : ControllerBase
{
    [Route("/api/postfiledownloadtasks/")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> AddPostFileDownloadTaskAsync(CreatePostFileDownloadTask command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Accepted();
    }

    [Route("/api/postfilestatus/initialize")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> InitializePostFileStatusAsync(InitializePostFileStatusCommand command,
    [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Accepted();
    }

    [Route("/api/postfilestatus/stopinitializing")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> StopInitializingPostFileStatusAsync(StopInitializingPostFileStatusCommand command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Accepted();
    }
}
