namespace AshLake.Services.Archiver.Application.Commands;

public record InitializePostRelationCommand(int Limit):IRequest;

public class InitializePostRelationCommandHandler : IRequestHandler<InitializePostRelationCommand>
{
    private readonly ICollectorService _collectorService;
    private readonly IPostRelationRepository _postRelationRepository;

    public InitializePostRelationCommandHandler(ICollectorService collectorService, IPostRelationRepository postRelationRepository)
    {
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task<Unit> Handle(InitializePostRelationCommand command, CancellationToken cancellationToken)
    {
        var postRelations = await _postRelationRepository.FindAsync(x => x.FileStatus == null, command.Limit);

        if (postRelations.Count() == 0) return Unit.Value;

        var updateList = new List<PostRelation>();
        var validExtList = new List<string>() { ".jpg", ".jpeg", ".png", ".gif" };

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

        return Unit.Value;
    }
}