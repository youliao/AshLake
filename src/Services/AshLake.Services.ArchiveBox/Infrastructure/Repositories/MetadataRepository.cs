namespace AshLake.Services.ArchiveBox.Infrastructure.Repositories;

public class MetadataRepository<T> : IMetadataRepository<T> where T : Mesadata
{
    private readonly IMongoDatabase _database;

    public MetadataRepository(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task AddAsync(T post)
    {
        await _database.GetEntityCollection<T>().InsertOneAsync(post);
    }

    public async Task DeleteAsync(string postId)
    {
        await _database.GetEntityCollection<T>().FindOneAndDeleteAsync(x => x.Id == postId);
    }

    public IQueryable<T> Query(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public async Task<T> SingleAsync(string postId)
    {
        return await _database.GetEntityCollection<T>()
            .Find(x => x.Id == postId)
            .Limit(1)
            .SingleOrDefaultAsync();
    }
}
