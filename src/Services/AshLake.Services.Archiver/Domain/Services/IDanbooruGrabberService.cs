namespace AshLake.Services.Archiver.Domain.Services;

public interface IDanbooruGrabberService
{
    Task<IEnumerable<BsonDocument>> GetPostMetadataList(int startId, int limit);

    Task<string?> GetPostObjectKey(int postId);

    Task<IEnumerable<BsonDocument>> GetTagMetadataList(int type);
}