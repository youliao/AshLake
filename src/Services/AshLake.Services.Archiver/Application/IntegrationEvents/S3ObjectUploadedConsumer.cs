using AshLake.Contracts.Collector.Events;

public class S3ObjectUploadedConsumer : IConsumer<S3ObjectUploaded>
{
    private readonly IPostRelationRepository _postRelationRepository;

    public S3ObjectUploadedConsumer(IPostRelationRepository postRelationRepository)
    {
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<S3ObjectUploaded> context)
    {
        var objectKey = context.Message.ObjectKey;

        var postRelation = new PostRelation()
        {
            Id = objectKey,
            FileStatus = PostFileStatus.InStock
        };

        await _postRelationRepository.UpdateFileStatus(postRelation);
    }
}
