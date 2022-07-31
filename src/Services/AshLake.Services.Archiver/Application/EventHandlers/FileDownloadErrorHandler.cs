namespace AshLake.Services.Archiver.Application.EventHandlers;

public class FileDownloadErrorHandler : IConsumer<FileDownloadError>
{
    private readonly IPostRelationRepository _postRelationRepository;

    public FileDownloadErrorHandler(IPostRelationRepository postRelationRepository)
    {
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<FileDownloadError> context)
    {
        var objectKey = context.Message.fileName;

        var postRelation = new PostRelation()
        {
            Id = objectKey,
            FileStatus = PostFileStatus.DownloadError
        };

        await _postRelationRepository.UpdateFileStatus(postRelation);
    }
}
