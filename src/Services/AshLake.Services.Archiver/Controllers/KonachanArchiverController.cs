namespace AshLake.Services.Archiver.Controllers;

public class KonachanArchiverController : ControllerBase
{
    [Route("/api/boorus/konachan/postmetadata/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostMetadata(int id,
        [FromServices] IMetadataRepository<Konachan,PostMetadata> repository)
    {
        var metadata = await repository.SingleAsync(id);
        if (metadata is null) return NotFound();

        return Ok(metadata.Data);
    }

    [Route("/api/boorus/konachan/postmetadata")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPostMetadataByRange(int rangeFrom, int rangeTo,
    [FromServices] IMetadataRepository<Konachan, PostMetadata> repository)
    {
        var list = await repository.FindAsync(x => x.Id>=rangeFrom && x.Id<= rangeTo) ?? new List<PostMetadata>();

        return Ok(list.Select(x => x.Data));
    }

    [Route("/api/boorus/konachan/addpostmetadatajobs")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> CreateAddPostMetadataJobs(CreateAddPostMetadataJobs<Konachan> command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);
        return Accepted();
    }

    [Route("/api/boorus/konachan/replacepostmetadatajobs")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> CreateReplacePostMetadataJobs(CreateReplacePostMetadataJobs<Konachan> command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);
        return Accepted();
    }
}
