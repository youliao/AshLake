namespace AshLake.Services.Yande.Domain.Posts;

public interface IPostRepository
{
    Task<Post> SingleAsync(int postId);
    Task AddPostAsync(Post post, CancellationToken cancellationToken = default);
    Task DeletePostAsync(int postId, CancellationToken cancellationToken = default);
}
