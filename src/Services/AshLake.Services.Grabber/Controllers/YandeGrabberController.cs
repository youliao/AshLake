namespace AshLake.Services.Grabber.Controllers;

public class YandeGrabberController : ControllerBase
{
    private readonly IYandeSourceSiteService _sourceSiteRepository;

    public YandeGrabberController(IYandeSourceSiteService sourceSiteRepository)
    {
        _sourceSiteRepository = sourceSiteRepository ?? throw new ArgumentNullException(nameof(sourceSiteRepository));
    }

    [Route("/api/sites/yande/postmetadata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult> GetPostsMetadataList(int startId, int limit, int page)
    {
        var list = await _sourceSiteRepository.GetMetadataListAsync(startId, limit, page);
        return Ok(list);
    }

    [Route("/api/sites/yande/postmetadata/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ActionResult> GetPostMetadata(int id)
    {
        var metadata = await _sourceSiteRepository.GetMetadataAsync(id);

        if (metadata is null) return NotFound();

        return Ok(metadata);
    }

    [Route("/api/sites/yande/postmetadata/lastest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult> GetLatestPost()
    {
        var post = await _sourceSiteRepository.GetLatestPostAsync();
        return Ok(post);
    }

    [Route("/api/sites/yande/postpreviews/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ActionResult> GetPostPreview(int id)
    {
        try
        {
            var image = await _sourceSiteRepository.GetPreviewAsync(id);
            Response.Headers.Add("postmd5", image.PostMD5);
            return File(image.Data, MimeMapping.MimeUtility.GetMimeMapping(image.Type.ToString()));
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch
        {
            throw;
        }
    }

    [Route("/api/sites/yande/postfiles/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ActionResult> GetPostFile(int id)
    {
        try
        {
            var image = await _sourceSiteRepository.GetFileAsync(id);
            Response.Headers.Add("postmd5", image.PostMD5);
            return File(image.Data, MimeMapping.MimeUtility.GetMimeMapping(image.Type.ToString()));
        }
        catch(ArgumentException)
        {
            return NotFound();
        }
        catch
        {
            throw;
        }
    }
}
