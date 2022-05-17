using AshLake.Services.Archiver.Application.Commands.AddPostMetadata;
using AshLake.Services.Archiver.Application.Commands.CreateJobsForAddOrUpdateMetadata;
using AshLake.Services.Archiver.Application.Queries.GetPostMetadata;
using System.Text.Json;

namespace AshLake.Services.ArchiveBox.Controllers;

public class YandeArchiveSiteController : ApiControllerBase
{
    [Route("/api/archivesites/yande/postmetadata")]
    [HttpPost]
    [ProducesResponseType(typeof(ArchiveStatus), StatusCodes.Status202Accepted)]
    public async Task<ActionResult<ArchiveStatus>> AddOrUpdateMetadata(AddYandePostMetadataCommand command)
    {
        var archiveStatus = await Mediator.Send(command);

        return AcceptedAtAction(nameof(GetPostMetadata),
                                new { id = command.PostId },
                                archiveStatus);
    }

    [Route("/api/archivesites/yande/postmetadata/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostMetadata(int id)
    {
        var query = new GetYandePostMetadataQuery() { PostId = id.ToString()};
        var postMetadata = await Mediator.Send(query);

        if (postMetadata is null) return NotFound();

        return Ok(postMetadata.Data);
    }

    [Route("/api/archivesites/yande/jobs/addorupdatemetadatajobs")]
    [HttpPost]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status202Accepted)]
    public async Task<ActionResult<IEnumerable<string>>> CreateJobsForAddOrUpdateMetadata(CreateYandeJobsForAddOrUpdateMetadataCommand command,
        [FromServices]IHttpClientFactory httpClientFactory)
    {
        var httpClient = httpClientFactory.CreateClient(BooruSites.Yande);

        var tasks = await Mediator.Send(command);
         
        return Ok();
    }
}
