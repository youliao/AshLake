namespace AshLake.Services.Archiver.Application.Commands;

public record CreateManyPostFileDownloadTasks(PostFileStatus Status ,int Limit):Request<CreateManyPostFileDownloadTasksResult>;
public record CreateManyPostFileDownloadTasksResult(IEnumerable<string> TaskIds);

public class CreateManyPostFileDownloadTasksHandler : IConsumer<CreateManyPostFileDownloadTasks>
{
    private readonly IBooruApiService _booruApiService;
    private readonly ICollectorAria2Service _collectorService;
    private readonly IPostRelationRepository _postRelationRepository;

    public CreateManyPostFileDownloadTasksHandler(IBooruApiService booruApiService, ICollectorAria2Service collectorService, IPostRelationRepository postRelationRepository)
    {
        _booruApiService = booruApiService ?? throw new ArgumentNullException(nameof(booruApiService));
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<CreateManyPostFileDownloadTasks> context)
    {
        var command = context.Message;

        var aria2Stat = await _collectorService.GetAria2GlobalStat();

        if (aria2Stat!.NumWaiting > 1000)
        {
            await context.RespondAsync(new CreateManyPostFileDownloadTasksResult(new List<string>()));
            return;
        }

        var postRelations = await _postRelationRepository.FindAsync(x => x.FileStatus == command.Status, command.Limit);

        if (postRelations.Count() == 0) {
            await context.RespondAsync(new CreateManyPostFileDownloadTasksResult(new List<string>()));
            return;
        }

        var taskIds = new List<string>();
        foreach (var item in postRelations)
        {
            var urls = new List<string>();

            if (item.DanbooruId != null)
                urls.Add(_booruApiService.GetPostFileDownloadLink<Danbooru>(item.Id));

            if (item.KonachanId != null)
                urls.Add(_booruApiService.GetPostFileDownloadLink<Konachan>(item.Id));

            if (item.YandereId != null)
                urls.Add(_booruApiService.GetPostFileDownloadLink<Yandere>(item.Id));

            if (urls.Count == 0) return;

            urls.Add(_booruApiService.GetPostFileDownloadLink<Gelbooru>(item.Id));

            var md5 = Path.GetFileNameWithoutExtension(item.Id);

            var taskId = await _collectorService.AddDownloadTask(urls, item.Id, md5);
            if (item.FileStatus != PostFileStatus.Downloading)
            {
                await _postRelationRepository.UpdateFileStatus(item with { FileStatus = PostFileStatus.Downloading });
            }

            taskIds.Add(taskId);
        }

        //await _postRelationRepository.UpdateFileStatus(postRelations.Select(x => x with { FileStatus = PostFileStatus.Downloading }));

        await context.RespondAsync(new CreateManyPostFileDownloadTasksResult(taskIds));
    }
}
