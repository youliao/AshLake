namespace AshLake.Services.YandeStore.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly YandeDbContext _dbContext;

    public PostRepository(YandeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<int> AddAsync(Post post)
    {
        _dbContext.Add(post);
        return _dbContext.SaveChangesAsync();
    }

    public Task<int> AddRangeAsync(IEnumerable<Post> post)
    {
        _dbContext.AddRange(post);
        return _dbContext.SaveChangesAsync();
    }

    public Task<int> UpdateAsync(Post post)
    {
        _dbContext.Update(post);
        return _dbContext.SaveChangesAsync();
    }

    public async Task<int> AddOrUpdateAsync(Post post)
    {
        var isExists = await _dbContext.Posts.AnyAsync(x => x.Id == post.Id);

        if (isExists)
            _dbContext.Update(post); 
        else
            _dbContext.Add(post);

        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(int postId)
    {
        var post = await _dbContext.Posts.SingleOrDefaultAsync(post => post.Id == postId) ?? throw new ArgumentNullException("Post does not exist");
        _dbContext.Remove(post);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<Post?> GetAsync(int postId)
    {
        return await _dbContext.Posts.SingleOrDefaultAsync(post => post.Id == postId);
    }

    public async Task<IEnumerable<int>> GetChildIdsAsync(int parentId)
    {
        return await _dbContext.Posts.Where(post => post.ParentId == parentId)
            .Select(post => post.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<object>> FindAsync(List<string> tags, List<PostRating> ratings,
        List<PostStatus> statuses)
    {
        var queryable = _dbContext.Posts.AsQueryable();

        if (tags.Count > 0) 
            queryable = queryable.Where(post => tags.All(t => post.Tags.Contains(t)));

        if (ratings.Count > 0) 
            queryable = queryable.Where(post => ratings.Contains(post.Rating));

        if (statuses.Count > 0) 
            queryable = queryable.Where(post => statuses.Contains(post.Status));

        return await queryable.Select(post => new { post.Id, post.Md5 }).ToListAsync();
    }
}
