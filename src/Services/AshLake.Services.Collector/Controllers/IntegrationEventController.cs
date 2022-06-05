using Microsoft.AspNetCore.Mvc;

namespace AshLake.Services.Collector.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IntegrationEventController : ControllerBase
{
    [HttpPost("YandePostMetadataAdded")]
    [Topic(DaprEventBus.DaprPubsubName, $"{nameof(PostMetadataAddedIntegrationEvent<ISouceSite>)}<{nameof(Yande)}>")]
    public Task HandleAsync(PostMetadataAddedIntegrationEvent<Yande> e)
    {
        foreach(var postId in e.PostIds)
        {
            BackgroundJob.Enqueue<YandeJob>(x => x.AddFile(int.Parse(postId)));
        }

        return Task.CompletedTask;
    }

    [HttpPost("YandePostMetadataUnchanged")]
    [Topic(DaprEventBus.DaprPubsubName, $"{nameof(PostMetadataUnchangedIntegrationEvent<ISouceSite>)}<{nameof(Yande)}>")]
    public Task HandleAsync(PostMetadataUnchangedIntegrationEvent<Yande> e)
    {
        foreach (var postId in e.PostIds)
        {
            BackgroundJob.Enqueue<YandeJob>(x => x.AddFile(int.Parse(postId)));
        }

        return Task.CompletedTask;
    }
}
