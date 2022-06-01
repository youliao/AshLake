namespace AshLake.Services.Collector.Integration.GrabberServices;

public interface IYandeGrabberService
{
    Task<PostPreview?> GetPostPreview(int postId);

    Task<PostFile?> GetPostFile(int postId);

    Task<string?> GetPostObjectKey(int postId);
}