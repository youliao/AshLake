namespace AshLake.Services.Archiver.Infrastructure.Services;

public interface IYandeGrabberService
{
    Task<IEnumerable<BsonDocument>> GetPostMetadataList(int startId, int limit);

    Task<string?> GetPostObjectKey(int postId);

    Task<IEnumerable<BsonDocument>> GetTagMetadataList(int type);
}