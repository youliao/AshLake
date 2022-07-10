namespace AshLake.Services.Archiver.Application.Commands;

public record SyncPostFileStatusCommand(int Limit);

public class SyncPostFileStatusCommandConsumer : IConsumer<SyncPostFileStatusCommand>
{
    private readonly IPostRelationRepository _postRelationRepository;
    private readonly IImgProxyService _imgProxyService;

    public SyncPostFileStatusCommandConsumer(IPostRelationRepository postRelationRepository, IImgProxyService imgProxyService)
    {
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
        _imgProxyService = imgProxyService ?? throw new ArgumentNullException(nameof(imgProxyService));
    }

    public async Task Consume(ConsumeContext<SyncPostFileStatusCommand> context)
    {
        var command = context.Message;
        var postRelations = await _postRelationRepository.FindAsync(x => x.FileStatus == null || x.FileStatus == PostFileStatus.None,command.Limit);

        var updateList = new List<PostRelation>();
        foreach(var item in postRelations)
        {
            var exists = await _imgProxyService.Exists(item.Id);
            
            if(exists)
            {
                updateList.Add(item with { FileStatus = PostFileStatus.InStock });
                continue;
            }

            if(item.FileStatus is null)
            {
                updateList.Add(item with {FileStatus = PostFileStatus.None });
            }
        }

        if (updateList.Count > 0)
        {
            await _postRelationRepository.UpdateFileStatus(updateList);
        }

    }
}
