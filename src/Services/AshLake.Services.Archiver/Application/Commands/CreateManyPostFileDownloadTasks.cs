namespace AshLake.Services.Archiver.Application.Commands;

public record CreateManyPostFileDownloadTasks(int Limit):Request<CreateManyPostFileDownloadTasksResult>;
public record CreateManyPostFileDownloadTasksResult(IEnumerable<string> taskIds);

public class CreateManyPostFileDownloadTasksHandler : IConsumer<CreateManyPostFileDownloadTasks>
{
    private readonly IBooruApiService _booruApiService;
    private readonly ICollectorService _collectorService;
    private readonly IPostRelationRepository _postRelationRepository;

    public CreateManyPostFileDownloadTasksHandler(IBooruApiService booruApiService, ICollectorService collectorService, IPostRelationRepository postRelationRepository)
    {
        _booruApiService = booruApiService ?? throw new ArgumentNullException(nameof(booruApiService));
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<CreateManyPostFileDownloadTasks> context)
    {
        var command = context.Message;

        var aria2Stat = await _collectorService.GetAria2GlobalStat();

        if (aria2Stat!.NumWaiting > 1000) return;

        var postRelations = await _postRelationRepository.FindAsync(x => x.FileStatus == PostFileStatus.None, command.Limit);

        if (postRelations.Count() == 0) return;

        var taskIds = new List<string>();
        foreach (var item in postRelations)
        {
            var urls = new List<string>();

            if (item.DanbooruId != null)
                urls.Add( _booruApiService.GetPostFileDownloadLink<Danbooru>(item.Id));

            if (item.KonachanId != null)
                urls.Add(_booruApiService.GetPostFileDownloadLink<Konachan>(item.Id));

            if (item.YandereId != null)
                urls.Add(_booruApiService.GetPostFileDownloadLink<Yandere>(item.Id));

            if (urls.Count == 0) return;

            urls.Add(_booruApiService.GetPostFileDownloadLink<Gelbooru>(item.Id));

            var md5 = Path.GetFileNameWithoutExtension(item.Id);

            var taskId = await _collectorService.AddDownloadTask(urls, item.Id, md5);
            taskIds.Add(taskId);
        }

        await _postRelationRepository.UpdateFileStatus(postRelations.Select(x => x with { FileStatus = PostFileStatus.Downloading }));

        await context.RespondAsync(new CreateManyPostFileDownloadTasksResult(taskIds));
    }
}
