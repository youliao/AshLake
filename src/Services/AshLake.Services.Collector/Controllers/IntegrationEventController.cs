﻿using Microsoft.AspNetCore.Mvc;

namespace AshLake.Services.Collector.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IntegrationEventController : ControllerBase
{
    [HttpPost("YanderePostMetadataAdded")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(YanderePostMetadataAddedIntegrationEvent))]
    public Task HandleAsync(YanderePostMetadataAddedIntegrationEvent e)
    {
        BackgroundJob.Enqueue<CollectingJob<Yande>>(x => x.AddFiles("yande", e.PostIds));
        return Task.CompletedTask;
    }

    [HttpPost("YanderePostMetadataModified")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(YanderePostMetadataModifiedIntegrationEvent))]
    public Task HandleAsync(YanderePostMetadataModifiedIntegrationEvent e)
    {
        BackgroundJob.Enqueue<CollectingJob<Yande>>(x => x.AddFiles("yande", e.PostIds));

        return Task.CompletedTask;
    }

    [HttpPost("DanbooruPostMetadataAdded")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(DanbooruPostMetadataAddedIntegrationEvent))]
    public Task HandleAsync(DanbooruPostMetadataAddedIntegrationEvent e)
    {
        BackgroundJob.Enqueue<CollectingJob<Danbooru>>(x => x.AddFiles("danbooru", e.PostIds));

        return Task.CompletedTask;
    }

    [HttpPost("DanbooruPostMetadataModified")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(DanbooruPostMetadataModifiedIntegrationEvent))]
    public Task HandleAsync(DanbooruPostMetadataModifiedIntegrationEvent e)
    {
        BackgroundJob.Enqueue<CollectingJob<Danbooru>>(x => x.AddFiles("danbooru", e.PostIds));

        return Task.CompletedTask;
    }

    [HttpPost("KonachanPostMetadataAdded")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(KonachanPostMetadataAddedIntegrationEvent))]
    public Task HandleAsync(KonachanPostMetadataAddedIntegrationEvent e)
    {
        BackgroundJob.Enqueue<CollectingJob<Konachan>>(x => x.AddFiles("konachan", e.PostIds));

        return Task.CompletedTask;
    }

    [HttpPost("KonachanPostMetadataModified")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(KonachanPostMetadataModifiedIntegrationEvent))]
    public Task HandleAsync(KonachanPostMetadataModifiedIntegrationEvent e)
    {
        BackgroundJob.Enqueue<CollectingJob<Konachan>>(x => x.AddFiles("konachan", e.PostIds));

        return Task.CompletedTask;
    }
}
