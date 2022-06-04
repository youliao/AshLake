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
    public Task HandleAsync(
        PostMetadataAddedIntegrationEvent<Yande> e)
    {
        foreach(var postId in e.PostIds)
        {
            BackgroundJob.Enqueue<PostJob>(x => x.AddPost(postId));
        }

        return Task.CompletedTask;
    }

    [HttpPost("YandePostMetadataModified")]
    [Topic(DaprEventBus.DaprPubsubName, $"{nameof(PostMetadataModifiedIntegrationEvent<ISouceSite>)}<{nameof(Yande)}>")]
    public Task HandleAsync(
    PostMetadataModifiedIntegrationEvent<Yande> e)
    {
        foreach (var postId in e.PostIds)
        {
            BackgroundJob.Enqueue<PostJob>(x => x.UpdatePost(postId));
        }

        return Task.CompletedTask;
    }
}
