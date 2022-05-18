namespace AshLake.Services.Archiver.Application.BackgroundJobs;

public class YandeMetadataJob
{
    private readonly IYandeMetadataRepository<PostMetadata> _repository;
    private readonly YandeGrabberService _grabberService;

    public YandeMetadataJob(IYandeMetadataRepository<PostMetadata> repository, YandeGrabberService grabberService)
    {
        _repository = repository;
        _grabberService = grabberService;
    }

    [Queue("metadata")]
    public async Task<int> AddOrUpdatePostMetadata(int startId, int endId, int limit)
    {
        var metadataList = await _grabberService.GetPostMetadataList(startId, limit);

        if (metadataList is null || metadataList.Count == 0) return 0;

        metadataList.RemoveAll(x => x["id"].AsInt32 >= endId);

        foreach (var item in metadataList)
        {
            var postMetadata = new PostMetadata() { Data = item };
            var status = await _repository.AddOrUpdateAsync(postMetadata);
        }

        return metadataList.Count;
    }
}
