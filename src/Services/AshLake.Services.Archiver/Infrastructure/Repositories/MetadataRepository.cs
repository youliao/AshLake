namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public abstract class MetadataRepository<T> : IMetadataRepository<T> where T : Metadata
{
    protected readonly MongoClient _mongoClient;
    protected abstract IMongoDatabase _database { get; }

    public MetadataRepository(MongoClient mongoClient)
    {
        _mongoClient = mongoClient;
    }

    public async Task<ArchiveStatus> AddOrUpdateAsync(T post)
    {
        var before = await _database.GetEntityCollection<T>().FindOneAndReplaceAsync(x => x.Id == post.Data["id"],
                                                                               post,
                                                                               new() { IsUpsert = true, ReturnDocument = ReturnDocument.Before });

        if (before is null) return ArchiveStatus.Added;

        if (post.Equals(before)) return ArchiveStatus.Untouched;

        return ArchiveStatus.Updated;
    }

    public async Task<T> DeleteAsync(string postId)
    {
        return await _database.GetEntityCollection<T>().FindOneAndDeleteAsync(x => x.Id == postId);
    }

    public async Task<T> SingleAsync(string postId)
    {
        return await _database.GetEntityCollection<T>()
            .Find(x => x.Id == postId)
            .Limit(1)
            .SingleOrDefaultAsync();
    }
}
