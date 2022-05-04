﻿
namespace AshLake.Services.Yande.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly YandeDbContext _dbContext;

    public PostRepository(YandeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> AddPostAsync(Post post, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(post, cancellationToken);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeletePostAsync(int postId, CancellationToken cancellationToken = default)
    {
        var post = await _dbContext.Set<Post>().FirstOrDefaultAsync(post => post.Id == postId) ?? throw new ArgumentNullException("Post does not exist");
        _dbContext.Posts.Remove(post);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<Post> SingleAsync(int postId)
    {
        return await _dbContext.Posts.SingleAsync(post => post.Id == postId);
    }
}
