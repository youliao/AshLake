using Microsoft.AspNetCore.Mvc;

namespace AshLake.Services.YandeStore.Controllers;

public class PostsController : ApiControllerBase
{
    [Route("/api/posts")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> AddPostAsync(AddPostCommand command)
    {
        await Mediator.Send(command);

        return CreatedAtAction(nameof(GetPostByIdAsync), command.PostId);
    }

    [Route("/api/posts/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostByIdAsync(int id)
    {
        var query = new GetPostByIdQuery(id);
        var post = await Mediator.Send(query);
        if(post is null) return NotFound();

        return Ok(post);
    }

    [Route("/api/posts")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPostsAsync([FromQuery]GetPostsQuery query)
    {
        var list = await Mediator.Send(query);

        return Ok(list);
    }
}
