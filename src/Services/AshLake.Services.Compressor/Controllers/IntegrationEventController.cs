using AshLake.Contracts.Collector.Events;
using AshLake.Services.Compressor.Application;
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
        BackgroundJob.Enqueue<CompressingJob>(x => x.AddPreview(e.ObjectKey));
        return Task.CompletedTask;
    }
}
