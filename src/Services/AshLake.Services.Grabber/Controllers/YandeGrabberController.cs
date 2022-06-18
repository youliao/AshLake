using AshLake.Services.Grabber.Infrastructure.Services;

namespace AshLake.Services.Grabber.Controllers;

public class YandeGrabberController : ControllerBase
{
    private readonly IYandeSourceSiteService _sourceSiteService;

    public YandeGrabberController(IYandeSourceSiteService sourceSiteService)
    {
        _sourceSiteService = sourceSiteService ?? throw new ArgumentNullException(nameof(sourceSiteService));
    }

    [Route("/api/sites/yande/postmetadata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult> GetPostMetadataList(int startId, int limit, int page)
    {
        var list = await _sourceSiteService.GetPostMetadataListAsync(startId, limit, page);
        return Ok(list);
    }

    [Route("/api/sites/yande/postmetadata/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ActionResult> GetPostMetadata(int id)
    {
        var metadata = await _sourceSiteService.GetPostMetadataAsync(id);

        if (metadata is null) return NotFound();

        return Ok(metadata);
    }

    [Route("/api/sites/yande/postmetadata/lastest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult> GetLatestPostMetadata()
    {
        var post = await _sourceSiteService.GetLatestPostMetadataAsync();
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

    [Route("/api/sites/yande/postfiles/{id:int}")]
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
        catch(ArgumentException)
        {
            return NotFound();
        }
        catch
        {
            throw;
        }
    }

    [Route("/api/sites/yande/postfilelinks/{id:int}")]
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

    [Route("/api/sites/yande/tagmetadata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult> GetTagMetadataList(int? type, int limit, int page)
    {
        var list = await _sourceSiteService.GetTagMetadataListAsync(type,limit, page);
        return Ok(list);
    }
}
