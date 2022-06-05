using Microsoft.AspNetCore.Mvc;

namespace AshLake.Services.YandeStore.Controllers;

public class PostsController : ApiControllerBase
{
    //[Route("/api/posts")]
    //[HttpPost]
    //[ProducesResponseType(StatusCodes.Status201Created)]
    //public async Task<ActionResult> AddPostAsync(AddPostCommand command)
    //{
    //    await Mediator.Send(command);

    //    return CreatedAtAction(nameof(GetPostByIdAsync), command.PostId);
    //}

    [Route("/api/posts/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostMetadataDto>> GetPostByIdAsync(int id)
    {
        var query = new GetPostByIdQuery(id);
        var dto = await Mediator.Send(query);
        if(dto is null) return NotFound();

        return Ok(dto);
    }

    [Route("/api/posts")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PostListItemDto>> GetPostsByKeysetPaginationAsync([FromQuery]GetPostsByKeysetPaginationQuery query)
    {
        var list = await Mediator.Send(query);

        return Ok(list);
    }
}
