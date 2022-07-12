namespace AshLake.Services.Archiver.Application.Commands;

public record CreatePostFileDownloadTask(string ObjectKey);

public class CreatePostFileDownloadTaskConsumer : IConsumer<CreatePostFileDownloadTask>
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

    public async Task Consume(ConsumeContext<CreatePostFileDownloadTask> context)
    {
        string objectKey = context.Message.ObjectKey;

        var postRelation = await _postRelationRepository.SingleAsync(objectKey);
        if (postRelation is null || postRelation.FileStatus != PostFileStatus.None) return;

        var linksdic = _booruApiService.GetPostFileLinks(objectKey);
        var urls = new List<string>();

        if (postRelation.DanbooruId != null)
            urls.Add(linksdic[nameof(Danbooru)]);

        if (postRelation.KonachanId != null)
            urls.Add(linksdic[nameof(Konachan)]);

        if (postRelation.YandereId != null)
            urls.Add(linksdic[nameof(Yandere)]);

        if (urls.Count == 0) return;
        var md5 = Path.GetFileNameWithoutExtension(objectKey);

        await _collectorService.AddDownloadTask(urls, objectKey, md5);

        await _postRelationRepository.UpdateFileStatus(postRelation with { FileStatus = PostFileStatus.Downloading });
    }
}
