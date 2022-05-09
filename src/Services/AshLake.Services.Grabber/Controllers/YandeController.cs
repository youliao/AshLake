namespace AshLake.Services.Grabber.Controllers;

public class YandeController : ApiControllerBase
{
    [Route("/api/yande/posts/metadata")]
    [HttpGet]
    public async Task<ActionResult<JsonArray>> GetPostsMetadata([FromQuery] GetYandePostsMetadataQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }
}
