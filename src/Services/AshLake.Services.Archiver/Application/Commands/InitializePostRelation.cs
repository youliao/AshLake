namespace AshLake.Services.Archiver.Application.Commands;

public record InitializePostRelation(int Limit);

public class InitializePostRelationHandler : IConsumer<InitializePostRelation>
{
    private readonly ICollectorRCloneService _collectorRCloneService;
    private readonly IPostRelationRepository _postRelationRepository;

    public InitializePostRelationHandler(ICollectorRCloneService collectorRCloneService, IPostRelationRepository postRelationRepository)
    {
        _collectorRCloneService = collectorRCloneService ?? throw new ArgumentNullException(nameof(collectorRCloneService));
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<InitializePostRelation> context)
    {
        var command = context.Message;
        var postRelations = await _postRelationRepository.FindAsync(x => x.FileStatus == null, command.Limit);

        if (postRelations.Count() == 0) return;

        var updateList = new List<PostRelation>();
        var validExtList = new List<string>() { ".jpg", ".jpeg", ".png", ".gif" };

        foreach (var item in postRelations)
        {
            if (!validExtList.Contains(Path.GetExtension(item.Id)))
            {
                updateList.Add(item with { FileStatus = PostFileStatus.Invalid });
                continue;
            }

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

        await _postRelationRepository.UpdateFileStatus(updateList);
    }
}