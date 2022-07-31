using AshLake.Contracts.Collector.Events;

namespace AshLake.Services.Archiver.Controllers;

[Route("api/integration")]
[ApiController]
public class IntegrationController : ControllerBase
{
    private readonly IMediator _mediator;

    public IntegrationController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    [HttpPost(nameof(FileDownloadCompleted))]
    [Topic(DaprInfo.DAPR_PUBSUB_NAME, nameof(FileDownloadCompleted))]
    public async Task Handle(FileDownloadCompleted @event)
    {
        await _mediator.Send(@event);
    }

    [HttpPost(nameof(YanderePostMetadataAdded))]
    [Topic(DaprInfo.DAPR_PUBSUB_NAME, nameof(YanderePostMetadataAdded))]
    public async Task Handle(YanderePostMetadataAdded @event)
    {
        await _mediator.Send(@event);
    }

    [HttpPost(nameof(DanbooruPostMetadataAdded))]
    [Topic(DaprInfo.DAPR_PUBSUB_NAME, nameof(DanbooruPostMetadataAdded))]
    public async Task Handle(DanbooruPostMetadataAdded @event)
    {
        await _mediator.Send(@event);
    }

    [HttpPost(nameof(KonachanPostMetadataAdded))]
    [Topic(DaprInfo.DAPR_PUBSUB_NAME, nameof(KonachanPostMetadataAdded))]
    public async Task Handle(KonachanPostMetadataAdded @event)
    {
        await _mediator.Send(@event);
    }
}
