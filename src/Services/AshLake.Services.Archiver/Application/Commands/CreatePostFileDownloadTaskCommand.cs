﻿namespace AshLake.Services.Archiver.Application.Commands;

public record CreatePostFileDownloadTaskCommand(string ObjectKey) : IRequest;

public class CreatePostFileDownloadTaskConsumer : IRequestHandler<CreatePostFileDownloadTaskCommand>
{
    private readonly IBooruApiService _booruApiService;
    private readonly ICollectorService _collectorService;
    private readonly IPostRelationRepository _postRelationRepository;

    public CreatePostFileDownloadTaskConsumer(IBooruApiService booruApiService, ICollectorService collectorService, IPostRelationRepository postRelationRepository)
    {
        _booruApiService = booruApiService ?? throw new ArgumentNullException(nameof(booruApiService));
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task<Unit> Handle(CreatePostFileDownloadTaskCommand command, CancellationToken cancellationToken)
    {
        string objectKey = command.ObjectKey;

        var postRelation = await _postRelationRepository.SingleAsync(objectKey);
        if (postRelation is null || postRelation.FileStatus != PostFileStatus.None) return Unit.Value;

        var linksdic = _booruApiService.GetPostFileLinks(objectKey);
        var urls = new List<string>();

        if (postRelation.DanbooruId != null)
            urls.Add(linksdic[nameof(Danbooru)]);

        if (postRelation.KonachanId != null)
            urls.Add(linksdic[nameof(Konachan)]);

        if (postRelation.YandereId != null)
            urls.Add(linksdic[nameof(Yandere)]);

        if (urls.Count == 0) return Unit.Value;
        var md5 = Path.GetFileNameWithoutExtension(objectKey);

        await _collectorService.AddDownloadTask(urls, objectKey, md5);

        await _postRelationRepository.UpdateFileStatus(postRelation with { FileStatus = PostFileStatus.Downloading });

        return Unit.Value;
    }
}
