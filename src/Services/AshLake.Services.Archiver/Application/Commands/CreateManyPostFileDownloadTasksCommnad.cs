namespace AshLake.Services.Archiver.Application.Commands;

public record CreateManyPostFileDownloadTasksCommnad(IEnumerable<string> ObjectKeys);

public class CreateManyPostFileDownloadTasksCommnadConsumer : IConsumer<CreateManyPostFileDownloadTasksCommnad>
{
    private readonly IBooruApiService _booruApiService;
    private readonly ICollectorService _collectorService;
    private readonly IPostRelationRepository _postRelationRepository;

    public CreateManyPostFileDownloadTasksCommnadConsumer(IBooruApiService booruApiService, ICollectorService collectorService, IPostRelationRepository postRelationRepository)
    {
        _booruApiService = booruApiService ?? throw new ArgumentNullException(nameof(booruApiService));
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<CreateManyPostFileDownloadTasksCommnad> context)
    {
        var objectKeys = context.Message.ObjectKeys;

        var postRelations = await _postRelationRepository.FindAsync(x=> objectKeys.Contains(x.Id));

        foreach(var item in postRelations)
        {
            if (item.FileStatus != PostFileStatus.None) continue;

            var linksdic = _booruApiService.GetPostFileLinks(item.Id);
            var urls = new List<string>();

            if (item.DanbooruId != null)
                urls.Add(linksdic[nameof(Danbooru)]);

            if (item.KonachanId != null)
                urls.Add(linksdic[nameof(Konachan)]);

            if (item.YandereId != null)
                urls.Add(linksdic[nameof(Yandere)]);

            if (urls.Count == 0) return;

            urls.Add(linksdic[nameof(Gelbooru)]);

            var md5 = Path.GetFileNameWithoutExtension(item.Id);

            await _collectorService.AddDownloadTask(urls, item.Id, md5);

            await _postRelationRepository.UpdateFileStatus(item with { FileStatus = PostFileStatus.Downloading });
        }
    }
}
