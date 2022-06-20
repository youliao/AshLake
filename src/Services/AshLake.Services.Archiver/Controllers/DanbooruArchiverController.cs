using System.Linq.Expressions;
using AshLake.Services.Archiver.Infrastructure.Extensions;
using AshLake.Services.Archiver.Application.Commands;
using MongoDB.Driver;

namespace AshLake.Services.Archiver.Controllers;

public class DanbooruArchiverController : ControllerBase
{
    [Route("/api/sites/danbooru/postmetadata/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostMetadataAsync(int id,
        [FromServices] IMetadataRepository<Danbooru,PostMetadata> repository)
    {
        var metadata = await repository.SingleAsync(id);
        if (metadata is null) return NotFound();

        return Ok(metadata.Data);
    }

    [Route("/api/sites/danbooru/postmetadata")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPostMetadataByRangeAsync(int rangeFrom, int rangeTo,
    [FromServices] IMetadataRepository<Danbooru, PostMetadata> repository)
    {
        var list = await repository.FindAsync(x => x.Id>=rangeFrom && x.Id<= rangeTo) ?? new List<PostMetadata>();

        return Ok(list.Select(x => x.Data));
    }

    [Route("/api/sites/danbooru/postobjectkeys/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPostobjectKeyAsync(int id,
        [FromServices] IMetadataRepository<Danbooru, PostMetadata> repository)
    {
        var metadata = await repository.SingleAsync(id);
        if (metadata is null) return NotFound();

        var postmd5 = metadata.Data[DanbooruPostMetadataKeys.md5].AsString;
        Guard.Against.NullOrWhiteSpace(postmd5);

        var fileExt = metadata.Data[DanbooruPostMetadataKeys.file_ext].AsString;
        Guard.Against.NullOrWhiteSpace(fileExt);

        var objectKey = $"{postmd5}.{fileExt}";

        return Ok(objectKey);
    }

    //[Route("/api/sites/danbooru/tagmetadata")]
    //[HttpGet]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<ActionResult> GetTagMetadataByTypeAsync(int type,
    //[FromServices] IMetadataRepository<Danbooru, TagMetadata> repository)
    //{
    //    var filter = Builders<TagMetadata>.Filter.Eq(DanbooruTagMetadataKeys.type, type);
    //    var list = await repository.FindAsync(filter);

    //    return Ok(list.Select(x => x.Data));
    //}

    [Route("/api/sites/danbooru/addpostmetadatajobs/batches")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreateAddPostMetadataJobsAsync(CreateAddPostMetadataJobsCommand command,
                [FromServices] IBackgroundJobClient backgroundJobClient)
    {
        var calls = new List<Expression<Func<PostMetadataJob<Danbooru>, Task>>>();

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

    [Route("/api/sites/danbooru/updatepostmetadatajobs/batches")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreateUpdatePostMetadataJobsAsync(CreateUpdatePostMetadataJobsCommand command,
            [FromServices] IBackgroundJobClient backgroundJobClient)
    {
        var calls = new List<Expression<Func<PostMetadataJob<Danbooru>, Task>>>();

        for (int i = command.StartId; i <= command.EndId; i += command.Step)
        {
            int startId = i;
            int endId = i + command.Step - 1;
            endId = Math.Min(endId, command.EndId);
            calls.Add(x => x.ReplacePostMetadata(startId, endId, command.Step));
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
        var calls = new List<Expression<Func<TagMetadataJob<Danbooru>, Task>>>();

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
