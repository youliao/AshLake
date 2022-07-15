using Microsoft.AspNetCore.Mvc;

namespace AshLake.Services.Collector.Controllers;

public class DownloadTaskController : ControllerBase
{
    [Route("/api/aria2/downloadtasks/{id:int}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> AddDownloadTasksAsync(AddDownloadTaskCommand command,
        [FromServices] Aria2NetClient aria2Client)
    {
        return Ok();
        //aria2Client.AddUriAsync(command.Urls);
        //if (metadata is null) return NotFound();

        //return Ok(metadata.Data);
    }
}
