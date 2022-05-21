namespace AshLake.Services.Grabber.Controllers;

public class YandeGrabberController : ApiControllerBase
{
    [Route("/api/sites/yande/postmetadata")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JsonObject>>> GetPostsMetadataList([FromQuery] GetYandePostsMetadataListQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [Route("/api/sites/yande/postmetadata/{id:int}")]
    [HttpGet]
    public async Task<ActionResult<JsonObject>> GetPostMetadata(int id)
    {
        var query = new GetYandePostMetadataQuery() { Id = id };
        var result = await Mediator.Send(query);

        if (result is null) return NotFound();

        return Ok(result);
    }

    [Route("/api/sites/yande/postpreviews/{id:int}")]
    [HttpGet]
    public async Task<FileResult> GetPostPreview(int id)
    {
        var query = new GetYandePostPreviewQuery() { Id = id };
        var stream = await Mediator.Send(query);

        return File(stream, MediaTypeNames.Image.Jpeg);
    }

    [Route("/api/sites/yande/postfiles/{id:int}")]
    [HttpGet]
    public async Task<FileResult> GetPostFile(int id)
    {
        var query = new GetYandePostFileQuery() { Id = id };
        (var stream, var fileExt) = await Mediator.Send(query);

        return File(stream, $"image/{fileExt}");
    }
}
