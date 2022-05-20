using AshLake.Services.Archiver.Application.Commands.CreateJobsForAddOrUpdateMetadata;

namespace AshLake.Services.Archiver.Controllers;

public class YandeArchiverController : ControllerBase
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
        var jobIdList = new List<string>();
        for (int i = command.StartPostId; i < command.EndPostId; i += command.Step)
        {
            var jobId = BackgroundJob.Enqueue<YandeJob>(x => x.AddOrUpdatePostMetadata(i, command.EndPostId, command.Step));
            jobIdList.Add(jobId);
        }

        return Ok(jobIdList);
    }

    [Route("/api/sites/yande/postpreviewjobs")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<string> CreatePostPreviewJobs(int postId)
    {
        var jobId = BackgroundJob.Enqueue<YandeJob>(x => x.AddOrUpdatePreview(postId));
        return Ok(jobId);
    }

    [Route("/api/sites/yande/postfilejobs")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<string> CreatePostFileJobs(int postId)
    {
        var jobId = BackgroundJob.Enqueue<YandeJob>(x => x.AddOrUpdateFile(postId));
        return Ok(jobId);
    }
}
