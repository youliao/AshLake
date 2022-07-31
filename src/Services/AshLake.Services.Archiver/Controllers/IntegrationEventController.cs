using AshLake.Contracts.Collector.Events;

namespace AshLake.Services.Archiver.Controllers;

[Route("api/integration")]
[ApiController]
public class IntegrationEventController : ControllerBase
{
    private readonly IMediator _mediator;

    public IntegrationEventController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    [HttpPost(nameof(S3ObjectUploaded))]
    [Topic(DaprInfo.DAPR_PUBSUB_NAME, nameof(S3ObjectUploaded))]
    public async Task HandleAsync(S3ObjectUploaded @event)
    {
        await _mediator.Send(@event);
    }

    [HttpPost(nameof(DanbooruPostMetadataAdded))]
    [Topic(DaprInfo.DAPR_PUBSUB_NAME, nameof(DanbooruPostMetadataAdded))]
    public async Task HandleAsync(DanbooruPostMetadataAdded @event)
    {
        await _mediator.Send(@event);
    }
}
