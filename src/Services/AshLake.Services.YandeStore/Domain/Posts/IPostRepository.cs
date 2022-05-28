namespace AshLake.Services.YandeStore.Domain.Posts;

public interface IPostRepository
{
    Task AddAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(int postId);
    Task<Post?> GetAsync(int postId);
    Task<IEnumerable<int>> GetChildIdsAsync(int parentId);
    Task<IEnumerable<object>> FindAsync(List<string> tags, List<PostRating> rating, List<PostStatus> status);
}
