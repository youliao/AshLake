using AshLake.Contracts.Collector.Events;
using Dapr;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace AshLake.Services.Compressor.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IntegrationEventController : ControllerBase
{
    [HttpPost("PostFileChanged")]
    [Topic(DaprEventBus.DaprPubsubName, nameof(PostFileChangedIntegrationEvent))]
    public Task HandleAsync(PostFileChangedIntegrationEvent e)
    {
        BackgroundJob.Enqueue<PostPreviewJob>(x => x.AddPreview(e.ObjectKey));
        return Task.CompletedTask;
    }
}
