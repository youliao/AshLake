namespace AshLake.Services.Collector.Application.Services;

public interface IYandeGrabberService
{
    Task<PostFile?> GetPostFile(int postId);

    Task<string?> GetPostObjectKey(int postId);
}