using AshLake.BuildingBlocks.EventBus;
using AshLake.Contracts.Archiver.Events;
using AshLake.Services.YandeStore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AshLake.Services.YandeStore.Controllers;

[Route("api/[controller]")]
public class IntegrationEventController : ApiControllerBase
{
    [HttpPost("YandePostMetadataAdded")]
    [Topic(DaprEventBus.DaprPubsubName, $"{nameof(PostMetadataAddedIntegrationEvent<ISouceSite>)}<{nameof(Yande)}>")]
    public async Task HandleAsync(
        PostMetadataAddedIntegrationEvent<Yande> e,
        [FromServices] IYandeArchiverService archiverService)
    {
        var metadata = await archiverService.GetPostMetadata(int.Parse(e.PostId));

        var command = metadata.Adapt<AddPostCommand>();

        await Mediator.Send(command);
    }
}
