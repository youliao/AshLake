using Microsoft.AspNetCore.Mvc;

namespace AshLake.Services.Collector.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IntegrationEventController : ControllerBase
{
    [HttpPost("YandePostMetadataAdded")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(YandePostMetadataAddedIntegrationEvent))]
    public Task HandleAsync(YandePostMetadataAddedIntegrationEvent e)
    {
        foreach (var postId in e.PostIds)
        {
            BackgroundJob.Enqueue<CollectingJob<Yande>>(x => x.AddOrUpdateFile(postId));
        }

        return Task.CompletedTask;
    }

    [HttpPost("DanbooruPostMetadataAdded")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(DanbooruPostMetadataAddedIntegrationEvent))]
    public Task HandleAsync(DanbooruPostMetadataAddedIntegrationEvent e)
    {
        foreach (var postId in e.PostIds)
        {
            BackgroundJob.Enqueue<CollectingJob<Danbooru>>(x => x.AddOrUpdateFile(postId));
        }

        return Task.CompletedTask;
    }

    //[HttpPost("YandePostMetadataUnchanged")]
    //[Topic(DaprEventBus.DaprPubsubName, $"{nameof(PostMetadataUnchangedIntegrationEvent<ISouceSite>)}<{nameof(Yande)}>")]
    //public Task HandleAsync(PostMetadataUnchangedIntegrationEvent<Yande> e)
    //{
    //    foreach (var postId in e.PostIds)
    //    {
    //        BackgroundJob.Enqueue<YandeJob>(x => x.AddFile(postId));
    //    }

    //    return Task.CompletedTask;
    //}
}
