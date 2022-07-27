namespace AshLake.Services.Archiver.Application.Commands;

public record ReputManyPostFiles(int Limit):Request<ReputManyPostFilesResult>;
public record ReputManyPostFilesResult(int AffectedCount);

public class ReputManyPostFilesHandler : IConsumer<ReputManyPostFiles>
{
    private readonly ICollectorService _collectorService;
    private readonly IPostRelationRepository _postRelationRepository;

    public ReputManyPostFilesHandler(ICollectorService collectorService, IPostRelationRepository postRelationRepository)
    {
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<ReputManyPostFiles> context)
    {
        var command = context.Message;

        var aria2Stat = await _collectorService.GetAria2GlobalStat();

        if (aria2Stat!.NumWaiting > 0 || aria2Stat.NumActive > 0)
        {
            await context.RespondAsync(new ReputManyPostFilesResult(0));
            return;
        }

        var postRelations = await _postRelationRepository.FindAsync(x => x.FileStatus == PostFileStatus.Downloading, command.Limit);

        if (postRelations.Count() == 0)
        {
            await context.RespondAsync(new ReputManyPostFilesResult(0));
            return;
        }

        var affectedCount = 0;
        foreach (var item in postRelations)
        {
            var sucessed = await _collectorService.ReputObject(item.Id);
            if(sucessed)
            {
                await _postRelationRepository.UpdateFileStatus(item with { FileStatus = PostFileStatus.InStock });
                affectedCount++;
            }
        }

        await context.RespondAsync(new ReputManyPostFilesResult(affectedCount));
    }
}
