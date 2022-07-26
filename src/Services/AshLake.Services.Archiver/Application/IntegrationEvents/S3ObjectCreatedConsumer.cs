using AshLake.Contracts.Collector.Events;

public class S3ObjectCreatedConsumer : IConsumer<S3ObjectCreated>
{
    private readonly IPostRelationRepository _postRelationRepository;

    public S3ObjectCreatedConsumer(IPostRelationRepository postRelationRepository)
    {
        _postRelationRepository = postRelationRepository ?? throw new ArgumentNullException(nameof(postRelationRepository));
    }

    public async Task Consume(ConsumeContext<S3ObjectCreated> context)
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
