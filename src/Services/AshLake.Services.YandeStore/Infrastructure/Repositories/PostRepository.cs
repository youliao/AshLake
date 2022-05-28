
namespace AshLake.Services.YandeStore.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly YandeDbContext _dbContext;

    public PostRepository(YandeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Post post)
    {
        _dbContext.Add(post);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Post post)
    {
        _dbContext.Update(post);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int postId)
    {
        var post = await _dbContext.Posts.SingleOrDefaultAsync(post => post.Id == postId) ?? throw new ArgumentNullException("Post does not exist");
        _dbContext.Remove(post);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Post?> GetAsync(int postId)
    {
        return await _dbContext.Posts.SingleOrDefaultAsync(post => post.Id == postId);
    }

    public async Task<IEnumerable<object>> FindAsync(List<string>? tags, List<PostRating>? ratings,
        List<PostStatus>? statuses)
    {
        var queryable = _dbContext.Posts.AsQueryable();

        if (tags != null && tags.Count > 0)
        {
            queryable = queryable.Where(post => tags.All(t => post.Tags.Contains(t)));
        }

        if (ratings != null && ratings.Count > 0)
        {
            queryable = queryable.Where(post => ratings.Contains(post.Rating));
        }

        if (statuses != null && statuses.Count > 0)
        {
            queryable = queryable.Where(post => statuses.Contains(post.Status));
        }

        return await queryable.Select(post => new { post.Id, post.Md5 }).ToListAsync();
    }
}
