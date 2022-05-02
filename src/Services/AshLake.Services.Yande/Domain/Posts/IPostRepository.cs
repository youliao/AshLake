namespace AshLake.Services.Yande.Domain.Posts;

public interface IPostRepository
{
    Task<Post> GetPostAsync(int postId);
    Task AddPostAsync(Post post);
    Task DeletePostAsync(int postId);
}
