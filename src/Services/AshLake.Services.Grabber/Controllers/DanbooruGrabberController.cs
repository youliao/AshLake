namespace AshLake.Services.Grabber.Controllers;

public class DanbooruGrabberController : ControllerBase
{
    private readonly IDanbooruSourceSiteService _sourceSiteService;

    public DanbooruGrabberController(IDanbooruSourceSiteService sourceSiteService)
    {
        _sourceSiteService = sourceSiteService ?? throw new ArgumentNullException(nameof(sourceSiteService));
    }

    [Route("/api/sites/danbooru/postmetadata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult> GetPostMetadataList(int startId, int limit, int page)
    {
        var list = await _sourceSiteService.GetPostMetadataListAsync(startId, limit, page);
        return Ok(list);
    }

    [Route("/api/sites/danbooru/postmetadata/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ActionResult> GetPostMetadata(int id)
    {
        var metadata = await _sourceSiteService.GetPostMetadataAsync(id);

        if (metadata is null) return NotFound();

        return Ok(metadata);
    }

    [Route("/api/sites/danbooru/postmetadata/lastest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult> GetLatestPostMetadata()
    {
        var post = await _sourceSiteService.GetLatestPostMetadataAsync();
        return Ok(post);
    }

    [Route("/api/sites/danbooru/postpreviews/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ActionResult> GetPostPreview(int id)
    {
        try
        {
            var image = await _sourceSiteService.GetPreviewAsync(id);
            Response.Headers.Add("X-MD5", image.PostMD5);
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

    [Route("/api/sites/danbooru/postfiles/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ActionResult> GetPostFile(int id)
    {
        try
        {
            var image = await _sourceSiteService.GetFileAsync(id);
            Response.Headers.Add("X-MD5", image.PostMD5);
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

    [Route("/api/sites/danbooru/postfilelinks/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ActionResult> GetPostFileLink(int id)
    {
        try
        {
            var imageLink = await _sourceSiteService.GetFileLinkAsync(id);
            return Ok(imageLink);
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

    [Route("/api/sites/danbooru/tagmetadata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult> GetTagMetadataList(int? type, int limit, int page)
    {
        var list = await _sourceSiteService.GetTagMetadataListAsync(type, limit, page);
        return Ok(list);
    }
}
