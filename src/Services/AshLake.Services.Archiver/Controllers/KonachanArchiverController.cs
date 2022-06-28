using System.Linq.Expressions;
using AshLake.Services.Archiver.Infrastructure.Extensions;
using AshLake.Services.Archiver.Application.Commands;
using MongoDB.Driver;

namespace AshLake.Services.Archiver.Controllers;

public class KonachanArchiverController : ControllerBase
{
    [Route("/api/boorus/konachan/postmetadata/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostMetadataAsync(int id,
        [FromServices] IMetadataRepository<Konachan,PostMetadata> repository)
    {
        var metadata = await repository.SingleAsync(id);
        if (metadata is null) return NotFound();

        return Ok(metadata.Data);
    }

    [Route("/api/boorus/konachan/postmetadata")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPostMetadataByRangeAsync(int rangeFrom, int rangeTo,
    [FromServices] IMetadataRepository<Konachan, PostMetadata> repository)
    {
        var list = await repository.FindAsync(x => x.Id>=rangeFrom && x.Id<= rangeTo) ?? new List<PostMetadata>();

        return Ok(list.Select(x => x.Data));
    }

    [Route("/api/boorus/konachan/postobjectkeys/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPostobjectKeyAsync(int id,
        [FromServices] IMetadataRepository<Konachan, PostMetadata> repository)
    {
        var metadata = await repository.SingleAsync(id);
        if (metadata is null) return NotFound();

        var postmd5 = metadata.Data[KonachanPostMetadataKeys.md5].AsString;
        Guard.Against.NullOrWhiteSpace(postmd5);


        if(!metadata.Data.TryGetValue(KonachanPostMetadataKeys.file_url,out var fileUrl)){
            return NotFound();
        }

        var fileExt = Path.GetExtension(fileUrl.AsString);
        Guard.Against.NullOrWhiteSpace(fileExt);

        var objectKey = $"{postmd5}{fileExt}";

        return Ok(objectKey);
    }

    //[Route("/api/boorus/konachan/tagmetadata")]
    //[HttpGet]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<ActionResult> GetTagMetadataByTypeAsync(int type,
    //[FromServices] IMetadataRepository<Konachan, TagMetadata> repository)
    //{
    //    var filter = Builders<TagMetadata>.Filter.Eq(KonachanTagMetadataKeys.type, type);
    //    var list = await repository.FindAsync(filter);

    //    return Ok(list.Select(x => x.Data));
    //}

    [Route("/api/boorus/konachan/addpostmetadatajobs/batches")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreateAddPostMetadataJobsAsync(CreateAddPostMetadataJobsCommand command,
                [FromServices] IBackgroundJobClient backgroundJobClient)
    {
        var calls = new List<Expression<Func<PostMetadataJob<Konachan>, Task>>>();

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

    [Route("/api/boorus/konachan/updatepostmetadatajobs/batches")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreateUpdatePostMetadataJobsAsync(CreateUpdatePostMetadataJobsCommand command,
            [FromServices] IBackgroundJobClient backgroundJobClient)
    {
        var calls = new List<Expression<Func<PostMetadataJob<Konachan>, Task>>>();

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

    [Route("/api/boorus/konachan/tagmetadatajobs/batches")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreateTagMetadataJobsAsync(CreateTagMetadataJobsCommand command,
            [FromServices] IBackgroundJobClient backgroundJobClient)
    {
        var calls = new List<Expression<Func<TagMetadataJob<Konachan>, Task>>>();

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
