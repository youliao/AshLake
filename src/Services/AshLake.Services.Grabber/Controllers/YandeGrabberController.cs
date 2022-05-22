namespace AshLake.Services.Grabber.Controllers;

public class YandeGrabberController : ControllerBase
{
    private readonly IYandeSourceSiteRepository _sourceSiteRepository;

    public YandeGrabberController(IYandeSourceSiteRepository sourceSiteRepository)
    {
        _sourceSiteRepository = sourceSiteRepository ?? throw new ArgumentNullException(nameof(sourceSiteRepository));
    }

    [Route("/api/sites/yande/postmetadata")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JsonObject>>> GetPostsMetadataList(int startId, int limit, int page)
    {
        var list = await _sourceSiteRepository.GetMetadataListAsync(startId, limit, page);
        return Ok(list);
    }

    [Route("/api/sites/yande/postmetadata/{id:int}")]
    [HttpGet]
    public async Task<ActionResult<JsonObject>> GetPostMetadata(int id)
    {
        var metadata = await _sourceSiteRepository.GetMetadataAsync(id);

        if (metadata is null) return NotFound();

        return Ok(metadata);
    }

    [Route("/api/sites/yande/postpreviews/{id:int}")]
    [HttpGet]
    public async Task<FileResult> GetPostPreview(int id)
    {
        var image = await _sourceSiteRepository.GetPreviewAsync(id);

        Response.Headers.Add("postmd5", image.PostMD5);
        return File(image.Data, $"image/{image.Type}".ToLower());
    }

    [Route("/api/sites/yande/postfiles/{id:int}")]
    [HttpGet]
    public async Task<FileResult> GetPostFile(int id)
    {
        var image = await _sourceSiteRepository.GetFileAsync(id);

        Response.Headers.Add("postmd5", image.PostMD5);
        return File(image.Data, $"image/{image.Type}".ToLower());
    }
}
