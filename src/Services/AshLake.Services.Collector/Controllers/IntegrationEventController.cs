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
            BackgroundJob.Enqueue<CollectingJob<Yande>>(x => x.AddYandeFile(postId));
        }

        return Task.CompletedTask;
    }

    [HttpPost("YandePostMetadataModified")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(YandePostMetadataModifiedIntegrationEvent))]
    public Task HandleAsync(YandePostMetadataModifiedIntegrationEvent e)
    {
        foreach (var postId in e.PostIds)
        {
            BackgroundJob.Enqueue<CollectingJob<Yande>>(x => x.AddYandeFile(postId));
        }

        return Task.CompletedTask;
    }

    [HttpPost("DanbooruPostMetadataAdded")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(DanbooruPostMetadataAddedIntegrationEvent))]
    public Task HandleAsync(DanbooruPostMetadataAddedIntegrationEvent e)
    {
        foreach (var postId in e.PostIds)
        {
            BackgroundJob.Enqueue<CollectingJob<Danbooru>>(x => x.AddDanbooruFile(postId));
        }

        return Task.CompletedTask;
    }

    [HttpPost("DanbooruPostMetadataModified")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(DanbooruPostMetadataModifiedIntegrationEvent))]
    public Task HandleAsync(DanbooruPostMetadataModifiedIntegrationEvent e)
    {
        foreach (var postId in e.PostIds)
        {
            BackgroundJob.Enqueue<CollectingJob<Danbooru>>(x => x.AddDanbooruFile(postId));
        }

        return Task.CompletedTask;
    }

    [HttpPost("KonachanPostMetadataAdded")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(KonachanPostMetadataAddedIntegrationEvent))]
    public Task HandleAsync(KonachanPostMetadataAddedIntegrationEvent e)
    {
        foreach (var postId in e.PostIds)
        {
            BackgroundJob.Enqueue<CollectingJob<Konachan>>(x => x.AddKonachanFile(postId));
        }

        return Task.CompletedTask;
    }

    [HttpPost("KonachanPostMetadataModified")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(KonachanPostMetadataModifiedIntegrationEvent))]
    public Task HandleAsync(KonachanPostMetadataModifiedIntegrationEvent e)
    {
        foreach (var postId in e.PostIds)
        {
            BackgroundJob.Enqueue<CollectingJob<Konachan>>(x => x.AddKonachanFile(postId));
        }

        return Task.CompletedTask;
    }
}
