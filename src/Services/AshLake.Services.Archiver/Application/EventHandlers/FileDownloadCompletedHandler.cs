namespace AshLake.Services.Archiver.Application.EventHandlers;

public class FileDownloadCompletedHandler : IConsumer<FileDownloadCompleted>
{
    private readonly IPostRelationRepository _postRelationRepository;

    public FileDownloadCompletedHandler(IPostRelationRepository postRelationRepository)
    {
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<FileDownloadCompleted> context)
    {
        var objectKey = context.Message.fileName;

        var postRelation = new PostRelation()
        {
            Id = objectKey,
            FileStatus = PostFileStatus.InStock
        };

        await _postRelationRepository.UpdateFileStatus(postRelation);
    }
}
