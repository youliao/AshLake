namespace AshLake.Services.Archiver.Integration.GrabberServices;

public interface IYandeGrabberService
{
    Task<IEnumerable<BsonDocument>> GetPostMetadataList(int startId, int limit);

    Task<Stream> GetPostPreview(int postId);

    Task<(Stream, string)> GetPostFile(int postId);
}