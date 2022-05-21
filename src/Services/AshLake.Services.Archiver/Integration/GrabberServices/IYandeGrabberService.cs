namespace AshLake.Services.Archiver.Integration.GrabberServices;

public interface IYandeGrabberService
{
    Task<IEnumerable<BsonDocument>> GetPostMetadataList(int startId, int limit);

    Task<byte[]> GetPostPreview(int postId);

    Task<(byte[], string)> GetPostFile(int postId);
}