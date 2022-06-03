namespace AshLake.Services.YandeStore.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly IDbContextFactory<YandeDbContext> _dbContextFactory;

    public PostRepository(IDbContextFactory<YandeDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    }

    public async Task AddAsync(Post post)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        await dbContext.AddAsync(post);
        await dbContext.SaveChangesAsync();
    }

    public Task UpdateAsync(Post post)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        dbContext.Update(post);
        return dbContext.SaveChangesAsync();
    }

    public async Task AddOrUpdateAsync(Post post)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var isExists = await dbContext.Posts.AnyAsync(x => x.Id == post.Id);

        if (isExists)
            dbContext.Update(post); 
        else 
            await dbContext.AddAsync(post);

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int postId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var post = await dbContext.Posts.SingleOrDefaultAsync(post => post.Id == postId) ?? throw new ArgumentNullException("Post does not exist");
        dbContext.Remove(post);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Post?> GetAsync(int postId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Posts.SingleOrDefaultAsync(post => post.Id == postId);
    }

    public async Task<IEnumerable<int>> GetChildIdsAsync(int parentId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return await dbContext.Posts.Where(post => post.ParentId == parentId)
            .Select(post => post.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<object>> FindAsync(List<string> tags, List<PostRating> ratings,
        List<PostStatus> statuses)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var queryable = dbContext.Posts.AsQueryable();

        if (tags.Count > 0) 
            queryable = queryable.Where(post => tags.All(t => post.Tags.Contains(t)));

        if (ratings.Count > 0) 
            queryable = queryable.Where(post => ratings.Contains(post.Rating));

        if (statuses.Count > 0) 
            queryable = queryable.Where(post => statuses.Contains(post.Status));

        return await queryable.Select(post => new { post.Id, post.Md5 }).ToListAsync();
    }
}
