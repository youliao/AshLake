namespace AshLake.Services.Archiver.Application.Commands;

public record RecheckDownloadingStatusCommand(int Limit) : IRequest<int>;

public class RecheckDownloadingStatusCommandHandler : IRequestHandler<RecheckDownloadingStatusCommand, int>
{
    private readonly ICollectorService _collectorService;
    private readonly IPostRelationRepository _postRelationRepository;

    public RecheckDownloadingStatusCommandHandler(ICollectorService collectorService, IPostRelationRepository postRelationRepository)
    {
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }


    public async Task<int> Handle(RecheckDownloadingStatusCommand command, CancellationToken cancellationToken)
    {
        var postRelations = await _postRelationRepository.FindAsync(x => x.FileStatus == PostFileStatus.Downloading, command.Limit);

        if (postRelations.Count() == 0) return 0;

        var updateList = new List<PostRelation>();

        foreach (var item in postRelations)
        {
            var exists = await _collectorService.ObjectExists(item.Id);

            if (!exists) continue;

            updateList.Add(item with { FileStatus = PostFileStatus.InStock });
        }

        if (updateList.Count() == 0) return 0;

        await _postRelationRepository.UpdateFileStatus(updateList);

        return updateList.Count;
    }
}