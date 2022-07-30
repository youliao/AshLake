namespace AshLake.Services.Archiver.Application.Commands;

public record CreatePostFileDownloadTask(string ObjectKey):Request<CreatePostFileDownloadTaskResult>;

public record CreatePostFileDownloadTaskResult(string TaskId);

public class CreatePostFileDownloadTaskHandler : IConsumer<CreatePostFileDownloadTask>
{
    private readonly IBooruApiService _booruApiService;
    private readonly ICollectorAria2Service _collectorService;
    private readonly IPostRelationRepository _postRelationRepository;

    public CreatePostFileDownloadTaskHandler(IBooruApiService booruApiService, ICollectorAria2Service collectorService, IPostRelationRepository postRelationRepository)
    {
        _booruApiService = booruApiService ?? throw new ArgumentNullException(nameof(booruApiService));
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<CreatePostFileDownloadTask> context)
    {
        string objectKey = context.Message.ObjectKey;

        var postRelation = await _postRelationRepository.SingleAsync(objectKey);
        if (postRelation is null) throw new Exception();

        var urls = new List<string>();

        if (postRelation.DanbooruId != null)
            urls.Add(_booruApiService.GetPostFileDownloadLink<Danbooru>(objectKey));

        if (postRelation.KonachanId != null)
            urls.Add(_booruApiService.GetPostFileDownloadLink<Konachan>(objectKey));

        if (postRelation.YandereId != null)
            urls.Add(_booruApiService.GetPostFileDownloadLink<Yandere>(objectKey));

        if (urls.Count == 0) return;

        urls.Add(_booruApiService.GetPostFileDownloadLink<Gelbooru>(objectKey));

        var md5 = Path.GetFileNameWithoutExtension(objectKey);

        var taskId = await _collectorService.AddDownloadTask(urls, objectKey, md5);
        await _postRelationRepository.UpdateFileStatus(postRelation with { FileStatus = PostFileStatus.Downloading });

        await context.RespondAsync(new CreatePostFileDownloadTaskResult(taskId));
    }
}
