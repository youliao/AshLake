﻿namespace AshLake.Services.Grabber.Controllers;

public class YandeSourceSiteController : ApiControllerBase
{
    [Route("/api/sourcesite/yande/postmetadata")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<JsonNode>>> GetPostsMetadataArray([FromQuery] GetYandePostsMetadataListQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [Route("/api/sourcesite/yande/postmetadata/{id:int}")]
    [HttpGet]
    public async Task<ActionResult<JsonNode>> GetPostMetadata(int id)
    {
        var query = new GetYandePostMetadataQuery() { Id = id };
        var result = await Mediator.Send(query);

        if (result is null) return NotFound();

        return Ok(result);
    }

    [Route("/api/sourcesite/yande/postpreviews/{id:int}")]
    [HttpGet]
    public async Task<FileResult> GetPostPreview(int id)
    {
        var query = new GetYandePostPreviewQuery() { Id = id };
        var stream = await Mediator.Send(query);

        return File(stream, MediaTypeNames.Image.Jpeg);
    }

    [Route("/api/sourcesite/yande/postfiles/{id:int}")]
    [HttpGet]
    public async Task<FileResult> GetPostFile(int id)
    {
        var query = new GetYandePostFileQuery() { Id = id };
        var stream = await Mediator.Send(query);

        return File(stream, MediaTypeNames.Image.Jpeg);
    }
}
