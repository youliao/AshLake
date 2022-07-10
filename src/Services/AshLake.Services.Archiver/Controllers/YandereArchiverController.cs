using MongoDB.Driver;

namespace AshLake.Services.Archiver.Controllers;

public class YandereArchiverController : ControllerBase
{
    [Route("/api/boorus/yandere/postmetadata/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostMetadataAsync(int id,
        [FromServices] IMetadataRepository<Yandere,PostMetadata> repository)
    {
        var metadata = await repository.SingleAsync(id);
        if (metadata is null) return NotFound();

        return Ok(metadata.Data);
    }

    [Route("/api/boorus/yandere/postmetadata")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPostMetadataByRangeAsync(int rangeFrom, int rangeTo,
    [FromServices] IMetadataRepository<Yandere, PostMetadata> repository)
    {
        var list = await repository.FindAsync(x => x.Id>=rangeFrom && x.Id<= rangeTo) ?? new List<PostMetadata>();

        return Ok(list.Select(x => x.Data));
    }


    [Route("/api/boorus/yandere/tagmetadata")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetTagMetadataByTypeAsync(int type,
    [FromServices] IMetadataRepository<Yandere, TagMetadata> repository)
    {
        var filter = Builders<TagMetadata>.Filter.Eq(YandereTagMetadataKeys.type, type);
        var list = await repository.FindAsync(filter);

        return Ok(list.Select(x => x.Data));
    }

    [Route("/api/boorus/yandere/addpostmetadatajobs")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> CreateAddPostMetadataJobsAsync(CreateAddPostMetadataJobsCommand<Yandere> command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);
        return Accepted();
    }

    [Route("/api/boorus/yandere/replacepostmetadatajobs")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> CreateUpdatePostMetadataJobsAsync(CreateReplacePostMetadataJobsCommand<Yandere> command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);
        return Accepted();
    }

    [Route("/api/boorus/yandere/addtagmetadatajobs/batches")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreateTagMetadataJobsAsync(CreateTagMetadataJobsCommand command)
    {
        var jobIdList = new List<string>();

        IEnumerable<int> tagTypes = command.TagTypes ?? new List<int>() { 0, 1, 3, 4, 5, 6 };

        foreach (var item in tagTypes)
        {
            var jobId = BackgroundJob.Enqueue<TagMetadataJob<Yandere>>(
                x => x.AddOrUpdateTagMetadata(nameof(Yandere).ToLower(), item));

            jobIdList.Add(jobId);
        }

        return Ok(jobIdList);
    }
}
