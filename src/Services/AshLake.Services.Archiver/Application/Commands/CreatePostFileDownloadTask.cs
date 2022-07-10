namespace AshLake.Services.Archiver.Application.Commands;

public record CreatePostFileDownloadTask(string ObjectKey);

public class CreatePostFileDownloadTaskConsumer : IConsumer<CreatePostFileDownloadTask>
{
    private readonly IPostRelationRepository _postRelationRepository;
    private readonly IBooruApiService _booruApiService;
    private readonly ICollectorService _collectorService;

    public CreatePostFileDownloadTaskConsumer(IPostRelationRepository postRelationRepository, IBooruApiService booruApiService, ICollectorService collectorService)
    {
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
        _booruApiService = booruApiService ?? throw new ArgumentNullException(nameof(booruApiService));
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
    }

    public async Task Consume(ConsumeContext<CreatePostFileDownloadTask> context)
    {
        string objectKey = context.Message.ObjectKey;

        //var postRelation = await _postRelationRepository.SingleAsync(objectKey);

        //if(postRelation == null) return;

        var dic = _booruApiService.GetPostFileLinks(objectKey);
        var urls = dic.Select(x => x.Value);
        var md5 = Path.GetFileNameWithoutExtension(objectKey);

        await _collectorService.AddDownloadTask(urls, objectKey, md5);
    }
}
