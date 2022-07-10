using System.Linq.Expressions;
using AshLake.Services.Archiver.Infrastructure.Extensions;
using AshLake.Services.Archiver.Application.Commands;
using MongoDB.Driver;
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
