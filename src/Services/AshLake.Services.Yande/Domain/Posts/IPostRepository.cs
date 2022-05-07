namespace AshLake.Services.Yande.Domain.Posts;

public interface IPostRepository
{
    Task AddAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(int postId);
    Task<Post?> GetAsync(int postId);
}
