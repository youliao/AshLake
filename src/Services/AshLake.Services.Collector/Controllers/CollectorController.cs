using AshLake.Services.Collector.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AshLake.Services.Collector.Controllers;

public class CollectorController : ControllerBase
{
    [Route("/api/postfiles/{objectKey}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostFileAsync(string objectKey,
        [FromServices] IS3ObjectRepositoty<PostFile> repositoty)
    {
        var data = await repositoty.GetDataAsync(objectKey);
        if (data is null) return NotFound();

        return File(data, MimeMapping.MimeUtility.GetMimeMapping(objectKey));
    }
}
