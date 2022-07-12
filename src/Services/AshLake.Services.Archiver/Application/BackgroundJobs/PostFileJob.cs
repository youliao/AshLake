namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class PostFileJob
{
    private readonly IPostRelationRepository _postRelationRepository;
    private readonly ICollectorService _collectorService;

    public PostFileJob(IPostRelationRepository postRelationRepository, ICollectorService collectorService)
    {
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
    }

    [Queue("common")]
    [AutomaticRetry(Attempts = 3)]
    public async Task<int> InitializePostFileStatus(int limit)
    {
        var postRelations = await _postRelationRepository.FindAsync(x => x.FileStatus == null, limit);

        if (postRelations.Count() == 0) return 0;

        var updateList = new List<PostRelation>();
        var validExtList = new List<string>() { ".jpg", ".png", ".gif" };

        foreach (var item in postRelations)
        {
            if (!validExtList.Contains(Path.GetExtension(item.Id)))
            {
                updateList.Add(item with { FileStatus = PostFileStatus.Invalid });
                continue;
            }

            var exists = await _collectorService.ObjectExists(item.Id);

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

        return updateList.Count;
    }
}
