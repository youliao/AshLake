namespace AshLake.Services.Yande.Domain.Posts;

public interface IPostRepository
{
    Task<Post> SingleAsync(int postId);
    Task<int> AddPostAsync(Post post, CancellationToken cancellationToken = default);
    Task<int> DeletePostAsync(int postId, CancellationToken cancellationToken = default);
}
