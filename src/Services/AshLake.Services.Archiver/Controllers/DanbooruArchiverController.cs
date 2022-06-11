using System.Linq.Expressions;
using AshLake.Services.Archiver.Infrastructure.Extensions;
using AshLake.Services.Archiver.Application.Commands;

namespace AshLake.Services.Archiver.Controllers;

public class DanbooruArchiverController : ControllerBase
{
    [Route("/api/sites/danbooru/postmetadata/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostMetadataAsync(int id,
        [FromServices] IMetadataRepository<Yande,PostMetadata> repository)
    {
        var metadata = await repository.SingleAsync(id.ToString());
        if (metadata is null) return NotFound();

        return Ok(metadata.Data);
    }

    [Route("/api/sites/danbooru/postmetadata")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPostMetadataByIdsAsync(string ids,
    [FromServices] IMetadataRepository<Yande, PostMetadata> repository)
    {
        if (string.IsNullOrWhiteSpace(ids)) return Ok(new List<PostMetadata>());

        var idArr = ids.Split(',');
        var list = await repository.FindAsync(x => idArr.Contains(x.Id)) ?? new List<PostMetadata>();

        return Ok(list.Select(x => x.Data));
    }

    [Route("/api/sites/danbooru/tagmetadata")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetTagMetadataByTypeAsync(int type,
    [FromServices] IMetadataRepository<Yande, TagMetadata> repository)
    {
        var filter = new MongoDB.Driver.FilterDefinitionBuilder<TagMetadata>().Eq(YandeTagMetadataKeys.type, type);
        var list = await repository.FindAsync(filter);

        return Ok(list.Select(x => x.Data));
    }

    [Route("/api/sites/danbooru/postmetadatajobs/batches")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreatePostMetadataJobsAsync(CreatePostMetadataJobsCommand command,
                [FromServices] IBackgroundJobClient backgroundJobClient)
    {
        var calls = new List<Expression<Func<DanbooruJob, Task>>>();

        for (int i = command.StartId; i <= command.EndId; i += command.Step)
        {
            int startId = i;
            int endId = i + command.Step - 1;
            endId = Math.Min(endId, command.EndId);
            calls.Add(x => x.AddOrUpdatePostMetadata(startId, endId, command.Step));
        }

        if (calls.Count == 0) return Ok();

        var jobIdList = backgroundJobClient.EnqueueSuccessively(calls);
        return Ok(jobIdList);
    }

    [Route("/api/sites/danbooru/tagmetadatajobs/batches")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreateTagMetadataJobsAsync(CreateTagMetadataJobsCommand command,
            [FromServices] IBackgroundJobClient backgroundJobClient)
    {
        var calls = new List<Expression<Func<DanbooruJob, Task>>>();

        IEnumerable<int> tagTypes = command.TagTypes ?? new List<int>() { 0, 1, 3, 4, 5, 6 };

        foreach(var item in tagTypes)
        {
            var type = item;
            calls.Add(x => x.AddOrUpdateTagMetadata(type));
        }

        if (calls.Count == 0) return Ok();

        var jobIdList = backgroundJobClient.EnqueueSuccessively(calls);
        return Ok(jobIdList);
    }
}
