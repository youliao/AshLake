﻿using System.Linq.Expressions;
using AshLake.Services.Archiver.Infrastructure.Extensions;
using AshLake.Services.Archiver.Application.Commands;
using MongoDB.Driver;

namespace AshLake.Services.Archiver.Controllers;

public class YandereArchiverController : ControllerBase
{
    [Route("/api/boorus/yandere/postmetadata/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPostMetadataAsync(int id,
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

    [Route("/api/boorus/yandere/postobjectkeys/{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPostobjectKeyAsync(int id,
        [FromServices] IMetadataRepository<Yandere, PostMetadata> repository)
    {
        var metadata = await repository.SingleAsync(id);
        if (metadata is null) return NotFound();

        var postmd5 = metadata.Data[YanderePostMetadataKeys.md5].AsString;
        Guard.Against.NullOrWhiteSpace(postmd5);

        var fileExt = metadata.Data[YanderePostMetadataKeys.file_ext].AsString;
        Guard.Against.NullOrWhiteSpace(fileExt);

        var objectKey = $"{postmd5}.{fileExt}";

        return Ok(objectKey);
    }


    [Route("/api/boorus/yandere/tagmetadata")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetTagMetadataByTypeAsync(int type,
    [FromServices] IMetadataRepository<Yandere, TagMetadata> repository)
    {
        var filter = Builders<TagMetadata>.Filter.Eq(YandereTagMetadataKeys.type, type);
        var list = await repository.FindAsync(filter);

        return Ok(list.Select(x => x.Data));
    }

    [Route("/api/boorus/yandere/addpostmetadatajobs/batches")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreateAddPostMetadataJobsAsync(CreateAddPostMetadataJobsCommand command,
                [FromServices] IBackgroundJobClient backgroundJobClient)
    {
        var calls = new List<Expression<Func<PostMetadataJob<Yandere>, Task>>>();

        for (int i = command.StartId; i <= command.EndId; i += command.Step)
        {
            int startId = i;
            int endId = i + command.Step - 1;
            endId = Math.Min(endId, command.EndId);
            calls.Add(x => x.AddOrUpdatePostMetadata(startId, endId, command.Step));
        }

        if (calls.Count == 0) return Ok();

        var jobIdList = backgroundJobClient.EnqueueSuccessively(calls);
        return Ok(jobIdList);
    }

    [Route("/api/boorus/yandere/updatepostmetadatajobs/batches")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreateUpdatePostMetadataJobsAsync(CreateUpdatePostMetadataJobsCommand command,
            [FromServices] IBackgroundJobClient backgroundJobClient)
    {
        var calls = new List<Expression<Func<PostMetadataJob<Yandere>, Task>>>();

        for (int i = command.StartId; i <= command.EndId; i += command.Step)
        {
            int startId = i;
            int endId = i + command.Step - 1;
            endId = Math.Min(endId, command.EndId);
            calls.Add(x => x.ReplacePostMetadata(startId, endId, command.Step));
        }

        if (calls.Count == 0) return Ok();

        var jobIdList = backgroundJobClient.EnqueueSuccessively(calls);
        return Ok(jobIdList);
    }

    [Route("/api/boorus/yandere/tagmetadatajobs/batches")]
    [HttpPost]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status202Accepted)]
    public ActionResult<List<string>> CreateTagMetadataJobsAsync(CreateTagMetadataJobsCommand command,
            [FromServices] IBackgroundJobClient backgroundJobClient)
    {
        var calls = new List<Expression<Func<TagMetadataJob<Yandere>, Task>>>();

        IEnumerable<int> tagTypes = command.TagTypes ?? new List<int>() { 0, 1, 3, 4, 5, 6 };

        foreach(var item in tagTypes)
        {
            var type = item;
            calls.Add(x => x.AddOrUpdateTagMetadata(type));
        }

        if (calls.Count == 0) return Ok();

        var jobIdList = backgroundJobClient.EnqueueSuccessively(calls);
        return Ok(jobIdList);
    }
}