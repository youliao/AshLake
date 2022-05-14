namespace AshLake.Services.ArchiveBox.Controllers;

public class YandeArchiveSiteController : ApiControllerBase
{
    [Route("/api/archivesites/yande/postmetadata")]
    [HttpPost]
    public async Task<ActionResult> AddOrUpdateMetadata(AddPostMetadataCommand command)
    {
        var archiveStatus = await Mediator.Send(command);

        return AcceptedAtAction(nameof(GetPostMetadata),
                                new { id = command.Id },
                                new { ArchiveStatus = archiveStatus });
    }

    [Route("/api/archivesites/yande/postmetadata/{id:int}")]
    [HttpGet]
    public async Task<ActionResult> GetPostMetadata(GetYandePostMetadataQuery query)
    {
        return Ok();
    }
}
