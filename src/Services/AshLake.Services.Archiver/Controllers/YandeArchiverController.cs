using AshLake.Services.Archiver.Application.Commands.CreateJobsForAddOrUpdateMetadata;
using System.Linq.Expressions;
using AshLake.Services.Archiver.Infrastructure.Extensions;

namespace AshLake.Services.Archiver.Controllers;

public class YandeArchiverController : ControllerBase
{
    [Route("/api/sites/yande/postmetadata/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostMetadata(int id,
        [FromServices] IMetadataRepository<Yande,PostMetadata> repository)
    {
        var metadata = await repository.SingleAsync(id.ToString());
        if (metadata is null) return NotFound();

        return Ok(metadata.Data);
    }

    [Route("/api/sites/yande/postmetadatajobs/batches")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreatePostMetadataJobs(CreatePostMetadataJobsCommand command,
                [FromServices] IBackgroundJobClient backgroundJobClient)
    {
        var calls = new List<Expression<Func<YandeJob, Task>>>();

        for (int i = command.StartId; i <= command.EndId; i += command.Step)
        {
            int startId = i;
            calls.Add(x => x.AddOrUpdatePostMetadata(startId, command.EndId, command.Step));
        }

        var jobIdList = backgroundJobClient.EnqueueSuccessively(calls);
        return Ok(jobIdList);
    }

    [Route("/api/sites/yande/postpreviewjobs")]
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
    public ActionResult<string> CreatePostPreviewJobs(int postId)
    {
        var jobId = BackgroundJob.Enqueue<YandeJob>(x => x.AddPreview(postId));
        return Ok(jobId);
    }

    [Route("/api/sites/yande/postfilejobs")]
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
    public ActionResult<string> CreatePostFileJobs(int postId)
    {
        var jobId = BackgroundJob.Enqueue<YandeJob>(x => x.AddFile(postId));
        return Ok(jobId);
    }

    [Route("/api/sites/yande/postfilejobs/batches")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreatePostFileJobs(int startId, int endId)
    {
        var jobIdList = new List<string>();
        for (int i = startId; i <= endId; i++)
        {
            var jobId = BackgroundJob.Enqueue<YandeJob>(x => x.AddFile(i));
            jobIdList.Add(jobId);
        }

        return Ok(jobIdList);
    }
}
