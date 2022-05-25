namespace AshLake.Services.Archiver.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class IntegrationEventController : ControllerBase
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    [HttpPost("YandePostMetadataAdded")]
    [Topic(DAPR_PUBSUB_NAME, "PostMetadataAddedIntegrationEvent<Yande>")]
    public Task HandleAsync(
        PostMetadataAddedIntegrationEvent<Yande> e,
        [FromServices] PostMetadataAddedIntegrationEventHandler handler)
        => handler.Handle(e);
}
