using AshLake.Contracts.Seedwork;
using AshLake.Contracts.Seedwork.Extensions;

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
        return await _database.GetEntityCollection<T>().FindOneAndReplaceAsync(x => x.Id == post.Id,
                                                                               post,
                                                                               new() { IsUpsert = true, ReturnDocument = ReturnDocument.Before });
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
