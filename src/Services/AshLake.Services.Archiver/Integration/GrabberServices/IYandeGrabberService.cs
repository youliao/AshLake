namespace AshLake.Services.Archiver.Integration.GrabberServices;

public interface IYandeGrabberService
{
    Task<List<BsonDocument>> GetPostMetadataList(int startId, int limit);

    Task<string> GetPostPreview(int postId);

    Task<(string, string)> GetPostFile(int postId);
}