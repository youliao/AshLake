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

    [Route("/api/postfilestatus/sync")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> SyncPostFileStatusAsync(SyncPostFileStatusCommand command,
    [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Accepted();
    }
}
