namespace AshLake.Services.Archiver.Application.Commands;

public record SyncPostFileStatusCommand(int Limit);

public class SyncPostFileStatusCommandConsumer : IConsumer<SyncPostFileStatusCommand>
{
    private readonly ICollectorService _collectorService;
    private readonly IPostRelationRepository _postRelationRepository;

    public SyncPostFileStatusCommandConsumer(ICollectorService collectorService, IPostRelationRepository postRelationRepository)
    {
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<SyncPostFileStatusCommand> context)
    {
        var command = context.Message;
        var postRelations = await _postRelationRepository.FindAsync(x => x.FileStatus == PostFileStatus.Downloading, command.Limit);

        if (postRelations.Count() == 0) return;

        var updateList = new List<PostRelation>();

        foreach (var item in postRelations)
        {
            var exists = await _collectorService.ObjectExists(item.Id);

            if (!exists) continue;

            updateList.Add(item with { FileStatus = PostFileStatus.InStock });
        }

        if (updateList.Count() == 0) return;

        await _postRelationRepository.UpdateFileStatus(updateList);
    }
}