namespace AshLake.Services.ArchiveBox.Controllers;

public class YandeArchiveSiteController : ApiControllerBase
{
    [Route("/api/archivesites/yande/postmetadata")]
    [HttpPost]
    [ProducesResponseType(typeof(ArchiveStatus), StatusCodes.Status202Accepted)]
    public async Task<ActionResult<ArchiveStatus>> AddOrUpdateMetadata(AddPostMetadataCommand command)
    {
        var archiveStatus = await Mediator.Send(command);

        return AcceptedAtAction(nameof(GetPostMetadata),
                                new { id = command.PostId },
                                archiveStatus);
    }

    [Route("/api/archivesites/yande/postmetadata/{id:int}")]
    [HttpGet]
    public async Task<ActionResult> GetPostMetadata(GetYandePostMetadataQuery query)
    {
        return Ok();
    }
}
