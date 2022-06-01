using AshLake.Services.Collector.Integration.EventHandling;
using Microsoft.AspNetCore.Mvc;

namespace AshLake.Services.Collector.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IntegrationEventController : ControllerBase
{
    [HttpPost("YandePostMetadataAdded")]
    [Topic(DaprEventBus.DaprPubsubName, $"{nameof(PostMetadataAddedIntegrationEvent<ISouceSite>)}<{nameof(Yande)}>")]
    public Task HandleAsync(
        PostMetadataAddedIntegrationEvent<Yande> e,
        [FromServices] PostMetadataAddedIntegrationEventHandler handler)
        => handler.Handle(e);
}
