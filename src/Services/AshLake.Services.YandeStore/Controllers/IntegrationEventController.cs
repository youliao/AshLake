using AshLake.BuildingBlocks.EventBus;
using AshLake.Contracts.Archiver.Events;
using AshLake.Services.YandeStore.Application.BackgroundJobs;
using Microsoft.AspNetCore.Mvc;

namespace AshLake.Services.YandeStore.Controllers;

[Route("api/[controller]")]
public class IntegrationEventController : ApiControllerBase
{
    [HttpPost("YandePostMetadataAdded")]
    [Topic(DaprEventBus.DaprPubsubName, $"{nameof(PostMetadataAddedIntegrationEvent<ISouceSite>)}<{nameof(Yande)}>")]
    public Task HandleAsync(PostMetadataAddedIntegrationEvent<Yande> e)
    {
        BackgroundJob.Enqueue<PostJob>(x => x.BulkAddPosts(e.PostIds));

        return Task.CompletedTask;
    }

    [HttpPost("YandePostMetadataModified")]
    [Topic(DaprEventBus.DaprPubsubName, $"{nameof(PostMetadataModifiedIntegrationEvent<ISouceSite>)}<{nameof(Yande)}>")]
    public Task HandleAsync(PostMetadataModifiedIntegrationEvent<Yande> e)
    {
        BackgroundJob.Enqueue<PostJob>(x => x.BulkUpdatePosts(e.PostIds));

        return Task.CompletedTask;
    }
}
