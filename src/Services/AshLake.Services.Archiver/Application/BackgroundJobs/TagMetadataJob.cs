﻿using MongoDB.Driver;

namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class TagMetadataJob<T> where T : Booru
{
    private readonly IMetadataRepository<T, TagMetadata> _tagMetadataRepository;
    private readonly IBooruApiService<T> _grabberService;
    private readonly IPublishEndpoint _publishEndpoint;

    public TagMetadataJob(IMetadataRepository<T, TagMetadata> tagMetadataRepository, IBooruApiService<T> grabberService, IPublishEndpoint publishEndpoint)
    {
        _tagMetadataRepository = tagMetadataRepository ?? throw new ArgumentNullException(nameof(tagMetadataRepository));
        _grabberService = grabberService ?? throw new ArgumentNullException(nameof(grabberService));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    [Queue("{0}")]
    [AutomaticRetry(Attempts = 3)]
    public async Task<dynamic> AddOrUpdateTagMetadata(string queue,int type)
    {
        var bsons = (await _grabberService.GetTagMetadataList(type)).ToList();

        var dataList = bsons.Select(x => new TagMetadata() { Data = x }).Reverse();
        var result = await _tagMetadataRepository.AddRangeAsync(dataList);

        if (result.AddedIds.Count > 0 || result.ModifiedIds.Count > 0)
            await _publishEndpoint.Publish(new DanbooruTagMetadataChanged(type));

        return new { Added = result.AddedIds.Count, Modified = result.ModifiedIds.Count, Unchanged = result.UnchangedIds.Count };
    }
}
