﻿namespace AshLake.Services.Archiver.Controllers;

public class DanbooruController : ControllerBase
{
    [Route("/api/boorus/danbooru/postmetadata/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostMetadata(int id,
        [FromServices] IMetadataRepository<Danbooru,PostMetadata> repository)
    {
        var metadata = await repository.SingleAsync(id);
        if (metadata is null) return NotFound();

        return Ok(metadata.Data);
    }

    [Route("/api/boorus/danbooru/postmetadata")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPostMetadataByRange(int rangeFrom, int rangeTo,
    [FromServices] IMetadataRepository<Danbooru, PostMetadata> repository)
    {
        var list = await repository.FindAsync(x => x.Id>=rangeFrom && x.Id<= rangeTo) ?? new List<PostMetadata>();

        return Ok(list.Select(x => x.Data));
    }

    [Route("/api/boorus/danbooru/addpostmetadatajobs")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> Handle(CreateAddPostMetadataJobs<Danbooru> command,
        [FromServices] IMediator mediator)
    {
        var result = await mediator.SendRequest(command);
        return Accepted(result);
    }

    [Route("/api/boorus/danbooru/replacepostmetadatajobs")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> Handle(CreateReplacePostMetadataJobs<Danbooru> command,
        [FromServices] IMediator mediator)
    {
        var result = await mediator.SendRequest(command);
        return Accepted();
    }
}
