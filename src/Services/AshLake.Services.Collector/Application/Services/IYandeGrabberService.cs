namespace AshLake.Services.Collector.Application.Services;

public interface IYandeGrabberService
{
    Task<PostPreview?> GetPostPreview(int postId);

    Task<PostFile?> GetPostFile(int postId);

    Task<string?> GetPostObjectKey(int postId);
}