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

        return CreatedAtAction(nameof(GetPostAsync), command.PostId);
    }

    [Route("/api/posts/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostAsync(int id)
    {
        var query = new GetPostQuery(id);
        var post = await Mediator.Send(query);
        if(post is null) return NotFound();

        return Ok(post);
    }
}
