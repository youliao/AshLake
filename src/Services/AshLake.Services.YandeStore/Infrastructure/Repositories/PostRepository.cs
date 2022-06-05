using MR.EntityFrameworkCore.KeysetPagination;

namespace AshLake.Services.YandeStore.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly YandeDbContext _dbContext;

    public PostRepository(YandeDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<int> AddAsync(Post entity)
    {
        _dbContext.Add(entity);
        return _dbContext.SaveChangesAsync();
    }

    public Task<int> AddRangeAsync(IEnumerable<Post> entity)
    {
        _dbContext.AddRangeAsync(entity.ToList());
        return _dbContext.SaveChangesAsync();
    }

    public Task<int> UpdateRangeAsync(IEnumerable<Post> entity)
    {
        _dbContext.UpdateRange(entity.ToList());
        return _dbContext.SaveChangesAsync();
    }

    public Task<int> UpdateAsync(Post entity)
    {
        _dbContext.Update(entity);
        return _dbContext.SaveChangesAsync();
    }

    public async Task<int> AddOrUpdateAsync(Post entity)
    {
        var isExists = await _dbContext.Posts.AnyAsync(x => x.Id == entity.Id);

        if (isExists)
            _dbContext.Update(entity);
        else
            _dbContext.Add(entity);

        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(int postId)
    {
        var entity = await _dbContext.Posts.SingleOrDefaultAsync(entity => entity.Id == postId) ?? throw new ArgumentNullException("Post does not exist");
        _dbContext.Remove(entity);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<Post?> GetAsync(int postId)
    {
        return await _dbContext.Posts.SingleOrDefaultAsync(entity => entity.Id == postId);
    }

    public async Task<IEnumerable<int>> GetChildIdsAsync(int parentId)
    {
        return await _dbContext.Posts.Where(entity => entity.ParentId == parentId)
            .Select(entity => entity.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<object>> FindAsync(List<string> tags, List<PostRating> ratings,
        List<PostStatus> statuses, PostOrderColumn orderColumn, int limit, int? referenceId)
    {
        var queryable = _dbContext.Posts.AsQueryable();

        if (tags.Count > 0)
            queryable = queryable.Where(entity => tags.All(tag => entity.Tags.Contains(tag)));

        if (ratings.Count > 0)
            queryable = queryable.Where(entity => ratings.Contains(entity.Rating));

        if (statuses.Count > 0)
            queryable = queryable.Where(entity => statuses.Contains(entity.Status));

        var reference = referenceId is null ? null : await GetAsync((int)referenceId);

        queryable = orderColumn switch
        {
            PostOrderColumn.ID => queryable.KeysetPaginateQuery(b => b.Ascending(entity => entity.Id), KeysetPaginationDirection.Forward, reference),
            PostOrderColumn.FILESIZE => queryable.KeysetPaginateQuery(b => b.Ascending(entity => entity.FileSize).Ascending(entity => entity.Id), KeysetPaginationDirection.Forward, reference),
            PostOrderColumn.SCORE => queryable.KeysetPaginateQuery(b => b.Ascending(entity => entity.Score).Ascending(entity => entity.Id), KeysetPaginationDirection.Forward, reference),
            PostOrderColumn.HEIGHT => queryable.KeysetPaginateQuery(b => b.Ascending(entity => entity.Height).Ascending(entity => entity.Id), KeysetPaginationDirection.Forward, reference),
            PostOrderColumn.WIDTH => queryable.KeysetPaginateQuery(b => b.Ascending(entity => entity.Width).Ascending(entity => entity.Id), KeysetPaginationDirection.Forward, reference),
            PostOrderColumn.ID_DESC => queryable.KeysetPaginateQuery(b => b.Descending(entity => entity.Id), KeysetPaginationDirection.Forward, reference),
            PostOrderColumn.FILESIZE_DESC => queryable.KeysetPaginateQuery(b => b.Descending(entity => entity.FileSize).Ascending(entity => entity.Id), KeysetPaginationDirection.Forward, reference),
            PostOrderColumn.SCORE_DESC => queryable.KeysetPaginateQuery(b => b.Descending(entity => entity.Score).Ascending(entity => entity.Id), KeysetPaginationDirection.Forward, reference),
            PostOrderColumn.HEIGHT_DESC => queryable.KeysetPaginateQuery(b => b.Descending(entity => entity.Height).Ascending(entity => entity.Id), KeysetPaginationDirection.Forward, reference),
            PostOrderColumn.WIDTH_DESC => queryable.KeysetPaginateQuery(b => b.Descending(entity => entity.Width).Ascending(entity => entity.Id), KeysetPaginationDirection.Forward, reference),
            _ => throw new NotSupportedException(orderColumn.ToString())
        };

        queryable = queryable.Take(limit);
        return await queryable.Select(entity => new { entity.Id, entity.Md5 }).ToListAsync();
    }
}
