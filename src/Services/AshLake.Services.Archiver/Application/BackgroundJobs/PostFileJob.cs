namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class PostFileJob
{
    private readonly IPostRelationRepository _postRelationRepository;
    private readonly IImgProxyService _imgProxyService;

    public PostFileJob(IPostRelationRepository postRelationRepository, IImgProxyService imgProxyService)
    {
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
        _imgProxyService = imgProxyService ?? throw new ArgumentNullException(nameof(imgProxyService));
    }

    [Queue("common")]
    [AutomaticRetry(Attempts = 3)]
    public async Task InitializePostFileStatus(int limit)
    {
        var postRelations = await _postRelationRepository.FindAsync(x => x.FileStatus == null, limit);

        if (postRelations.Count() == 0) return;

        var updateList = new List<PostRelation>();
        var validExtList = new List<string>() { ".jpg", ".png", "gif" };

        foreach (var item in postRelations)
        {
            if (!validExtList.Contains(Path.GetExtension(item.Id)))
            {
                updateList.Add(item with { FileStatus = PostFileStatus.Invalid });
                continue;
            }

            var exists = await _imgProxyService.Exists(item.Id);

            if (exists)
            {
                updateList.Add(item with { FileStatus = PostFileStatus.InStock });
            }
            else
            {
                updateList.Add(item with { FileStatus = PostFileStatus.None });
            }
        }

        await _postRelationRepository.UpdateFileStatus(updateList);

    }
}
