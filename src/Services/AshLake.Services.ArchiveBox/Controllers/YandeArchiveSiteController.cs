namespace AshLake.Services.ArchiveBox.Controllers;

public class YandeArchiveSiteController : ApiControllerBase
{
    [Route("/api/archivesites/yande/postmetadata")]
    [HttpPost]
    public async Task<ActionResult> AddPostMetadata(AddPostMetadataCommand command)
    {
        var before = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetPostMetadata), new { id = command.Id });
    }

    [Route("/api/archivesites/yande/postmetadata")]
    [HttpGet]
    public async Task<ActionResult> GetPostMetadata(GetYandePostMetadataQuery query)
    {
        return Ok();
    }
}
