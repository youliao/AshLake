namespace AshLake.Services.Archiver.Application.Commands;

public record CreatePostFileDownloadTaskCommand(string ObjectKey):Request<CreatePostFileDownloadTaskResult>;

public record CreatePostFileDownloadTaskResult(string TaskId);

public class CreatePostFileDownloadTaskCommnadHandler : IConsumer<CreatePostFileDownloadTaskCommand>
{
    private readonly IBooruApiService _booruApiService;
    private readonly ICollectorService _collectorService;
    private readonly IPostRelationRepository _postRelationRepository;

    public CreatePostFileDownloadTaskCommnadHandler(IBooruApiService booruApiService, ICollectorService collectorService, IPostRelationRepository postRelationRepository)
    {
        _booruApiService = booruApiService ?? throw new ArgumentNullException(nameof(booruApiService));
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<CreatePostFileDownloadTaskCommand> context)
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
