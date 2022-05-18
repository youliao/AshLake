using AshLake.Services.Archiver.Application.Commands.CreateJobsForAddOrUpdateMetadata;

namespace AshLake.Services.Archiver.Controllers;

public class YandeArchiverController : ApiControllerBase
{
    [Route("/api/sites/yande/postmetadata/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostMetadata(int id,
        [FromServices] IYandeMetadataRepository<PostMetadata> repository)
    {
        var metadata = await repository.SingleAsync(id.ToString());

        if (metadata is null) return NotFound();

        return Ok(metadata.Data);
    }

    [Route("/api/sites/yande/postmetadatajobs")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreatePostMetadataJobs(CreatePostMetadataJobsCommand command)
    {
        var step = 100;
        var jobIdList = new List<string>();
        for (int i = command.StartPostId; i < command.EndPostId; i += step)
        {
            var jobId = BackgroundJob.Enqueue<YandeMetadataJob>(x => x.AddOrUpdatePostMetadata(i, step));
            jobIdList.Add(jobId);
        }

        return Ok(jobIdList);
    }
}
