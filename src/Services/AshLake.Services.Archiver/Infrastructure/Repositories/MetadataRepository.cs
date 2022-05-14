using AshLake.Contracts.Seedwork;

namespace AshLake.Services.ArchiveBox.Infrastructure.Repositories;

public abstract class MetadataRepository<T> : IMetadataRepository<T> where T : Mesadata
{
    protected readonly MongoClient _mongoClient;
    protected abstract IMongoDatabase _database { get; }

    public MetadataRepository(MongoClient mongoClient)
    {
        _mongoClient = mongoClient;
    }

    public async Task<T> FindOneAndReplaceAsync(T post)
    {
        var opt = new FindOneAndReplaceOptions<T, T>() { IsUpsert = true, ReturnDocument = ReturnDocument.Before};
        return await _database.GetEntityCollection<T>().FindOneAndReplaceAsync<T>(x=>x.Id == post.Id, post, opt);
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
