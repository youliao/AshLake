﻿namespace AshLake.Services.Archiver.Controllers;

public class CommonController : ControllerBase
{
    [Route("/api/postfiledownloadtasks/")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> AddPostFileDownloadTaskAsync(CreatePostFileDownloadTask command,
        [FromServices] IMediator mediator)
    {
        var taskId = await mediator.SendRequest(command);

        return Accepted(taskId);
    }

    [Route("/api/recurringjobs/initializepostrelation")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> InitializePostFileStatusAsync(StartInitializingPostRelation command,
    [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Accepted();
    }

    [Route("/api/recurringjobs/initializepostrelation")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> StopInitializingPostFileStatusAsync(StopInitializingPostRelation command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Accepted();
    }

    [Route("/api/recurringjobs/downloadmanypostfiles")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> StartDownloadingManyPostFilesAsync(StartDownloadingManyPostFiles command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Accepted();
    }

    [Route("/api/recurringjobs/downloadmanypostfiles")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> StopDownloadingManyPostFilesAsync(StopDownloadingManyPostFiles command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);

        return Accepted();
    }

    [Route("/api/postrelations/recheckpostrelationdownloadingstatus")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> RecheckPostRelationDownloadingStatusAsync(RecheckPostRelationDownloadingStatus command,
        [FromServices] IMediator mediator)
    {
        var result = await mediator.SendRequest(command);

        return Accepted(result);
    }

    [Route("/api/postrelations/{objectKey}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostRelation>> GetPostRelationAsync(string objectKey,
        [FromServices] IPostRelationRepository repository)
    {
        var postRelation = await repository.SingleAsync(objectKey);

        if (postRelation is null) return NotFound();

        return Ok(postRelation);
    }

    [Route("/api/postrelationscount")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetPostRelationsCountAsync([FromQuery]PostFileStatus? status,
        [FromServices] IPostRelationRepository repository)
    {
        var count = await repository.CountAsync(x => x.FileStatus == status);

        return Ok(count);
    }

    [Route("/api/aria2globalstat")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Aria2NET.GlobalStatResult>> GetAria2GlobalStatAsync([FromServices] ICollectorAria2Service collectorService)
    {
        var stat = await collectorService.GetGlobalStat();

        return Ok(stat);
    }
}
