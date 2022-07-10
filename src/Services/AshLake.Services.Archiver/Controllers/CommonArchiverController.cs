using MassTransit.Mediator;

namespace AshLake.Services.Archiver.Controllers;

public class CommonArchiverController : ControllerBase
{
    [Route("/api/postfiledownloadtask/")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPostMetadataAsync(CreatePostFileDownloadTask command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Ok();
    }
}
