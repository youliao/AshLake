using Microsoft.AspNetCore.Mvc;

namespace AshLake.Services.Collector.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IntegrationEventController : ControllerBase
{
    [HttpPost("YandePostMetadataAdded")]
    [Topic(DaprEventBus.DaprPubsubName, $"{nameof(PostMetadataAddedIntegrationEvent<ISouceSite>)}<{nameof(Yande)}>")]
    public Task HandleAsync(
        PostMetadataAddedIntegrationEvent<Yande> e)
    {
        BackgroundJob.Enqueue<YandeJob>(x => x.AddFile(int.Parse(e.PostId)));
        BackgroundJob.Enqueue<YandeJob>(x => x.AddPreview(int.Parse(e.PostId)));
        return Task.CompletedTask;
    }
}
