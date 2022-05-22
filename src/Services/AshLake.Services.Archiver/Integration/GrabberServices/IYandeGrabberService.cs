namespace AshLake.Services.Archiver.Integration.GrabberServices;

public interface IYandeGrabberService
{
    Task<IEnumerable<BsonDocument>> GetPostMetadataList(int startId, int limit);

    Task<PostPreview> GetPostPreview(int postId);

    Task<PostFile> GetPostFile(int postId);
}