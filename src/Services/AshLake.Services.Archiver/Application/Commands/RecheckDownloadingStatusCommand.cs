namespace AshLake.Services.Archiver.Application.Commands;

public record RecheckDownloadingStatusCommand(int Limit):Request<RecheckDownloadingStatusResult>;
public record RecheckDownloadingStatusResult(int AffectedRows);
public class RecheckDownloadingStatusCommandHandler : IConsumer<RecheckDownloadingStatusCommand>
{
    private readonly ICollectorService _collectorService;
    private readonly IPostRelationRepository _postRelationRepository;

    public RecheckDownloadingStatusCommandHandler(ICollectorService collectorService, IPostRelationRepository postRelationRepository)
    {
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<RecheckDownloadingStatusCommand> context)
    {
        var command = context.Message;

        var postRelations = await _postRelationRepository.FindAsync(x => x.FileStatus == PostFileStatus.Downloading, command.Limit);

        if (postRelations.Count() == 0)
        {
            await context.RespondAsync(new RecheckDownloadingStatusResult(0));
            return;
        }

        var updateList = new List<PostRelation>();

        foreach (var item in postRelations)
        {
            var exists = await _collectorService.ObjectExists(item.Id);

            if (!exists) continue;

            updateList.Add(item with { FileStatus = PostFileStatus.InStock });
        }

        if (updateList.Count() == 0)
        {
            await context.RespondAsync(new RecheckDownloadingStatusResult(0));
            return;
        }

        await _postRelationRepository.UpdateFileStatus(updateList);

        await context.RespondAsync(new RecheckDownloadingStatusResult(updateList.Count));
    }
}