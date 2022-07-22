using MongoDB.Driver;

namespace AshLake.Services.Archiver.Controllers;

public class YandereArchiverController : ControllerBase
{
    [Route("/api/boorus/yandere/postmetadata/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostMetadata(int id,
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
    public async Task<ActionResult> GetTagMetadataByType(int type,
    [FromServices] IMetadataRepository<Yandere, TagMetadata> repository)
    {
        var filter = Builders<TagMetadata>.Filter.Eq(Yandere.TagMetadataKeys.type, type);
        var list = await repository.FindAsync(filter);

        return Ok(list.Select(x => x.Data));
    }

    [Route("/api/boorus/yandere/addpostmetadatajobs")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> CreateAddPostMetadataJobs(CreateAddPostMetadataJobsCommand<Yandere> command,
        [FromServices] IMediator mediator)
    {
        var result = await mediator.SendRequest(command);
        return Accepted(result);
    }

    [Route("/api/boorus/yandere/replacepostmetadatajobs")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> CreateReplacePostMetadataJobs(CreateReplacePostMetadataJobsCommand<Yandere> command,
        [FromServices] IMediator mediator)
    {
        var result = await mediator.SendRequest(command);
        return Accepted(result);
    }
}
