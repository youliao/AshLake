namespace AshLake.Services.Archiver.Application.Commands;

public record RecheckPostRelationDownloadingStatus(int Limit):Request<RecheckPostRelationDownloadingStatusResult>;
public record RecheckPostRelationDownloadingStatusResult(int AffectedRows);
public class RecheckPostRelationDownloadingStatusHandler : IConsumer<RecheckPostRelationDownloadingStatus>
{
    private readonly ICollectorRCloneService _collectorRCloneService;
    private readonly IPostRelationRepository _postRelationRepository;

    public RecheckPostRelationDownloadingStatusHandler(ICollectorRCloneService collectorRCloneService, IPostRelationRepository postRelationRepository)
    {
        _collectorRCloneService = collectorRCloneService ?? throw new ArgumentNullException(nameof(collectorRCloneService));
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<RecheckPostRelationDownloadingStatus> context)
    {
        var command = context.Message;

        var postRelations = await _postRelationRepository.FindAsync(x => x.FileStatus == PostFileStatus.Downloading, command.Limit);

        if (postRelations.Count() == 0)
        {
            await context.RespondAsync(new RecheckPostRelationDownloadingStatusResult(0));
            return;
        }

        var updateList = new List<PostRelation>();

        foreach (var item in postRelations)
        {
            var exists = await _collectorRCloneService.ObjectExists(item.Id);

            if (exists)
            {
                updateList.Add(item with { FileStatus = PostFileStatus.InStock });
            }
            else
            {
                updateList.Add(item with { FileStatus = PostFileStatus.None });
            }
        }

        if (updateList.Count() == 0)
        {
            await context.RespondAsync(new RecheckPostRelationDownloadingStatusResult(0));
            return;
        }

        await _postRelationRepository.UpdateFileStatus(updateList);

        await context.RespondAsync(new RecheckPostRelationDownloadingStatusResult(updateList.Count));
    }
}