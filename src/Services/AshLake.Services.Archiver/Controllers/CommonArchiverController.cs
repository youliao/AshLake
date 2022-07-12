namespace AshLake.Services.Archiver.Controllers;

public class CommonArchiverController : ControllerBase
{
    [Route("/api/postfiledownloadtasks/")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> AddPostFileDownloadTaskAsync(CreatePostFileDownloadTaskCommand command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Accepted();
    }

    [Route("/api/postfilestatus/initialize")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> InitializePostFileStatusAsync(StartInitializingPostFileStatusCommand command,
    [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Accepted();
    }

    [Route("/api/postfilestatus/initialize")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> StopInitializingPostFileStatusAsync(StopInitializingPostFileStatusCommand command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Accepted();
    }

    [Route("/api/postfiles/download")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> StartDownloadingManyPostFilesAsync(StartDownloadingManyPostFilesCommand command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Accepted();
    }

    [Route("/api/postfiles/download")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> StopDownloadingManyPostFilesAsync(StopDownloadingManyPostFilesCommand command,
    [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Accepted();
    }
}
