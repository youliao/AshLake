using AshLake.Services.Archiver.Infrastructure.Extensions;
using MongoDB.Driver;

namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public class MetadataRepository<TSouceSite, TMetadata> : IMetadataRepository<TSouceSite, TMetadata>
    where TSouceSite : ISouceSite
    where TMetadata : Metadata
{
    private readonly IMongoDatabase _database;

    public MetadataRepository(MongoClient mongoClient)
    {
        var databaseName = typeof(TSouceSite).Name;
        _database = mongoClient.GetDatabase(databaseName);
    }

    public async Task<EntityState> AddOrUpdateAsync(TMetadata post)
    {
        var before = await _database.GetEntityCollection<TMetadata>()
            .FindOneAndReplaceAsync(x => x.Id == post.Data["id"],
                                   post,
                                   new() { IsUpsert = true, ReturnDocument = ReturnDocument.Before });

        if (before is null) return EntityState.Added;

        if (post.Equals(before)) return EntityState.Unchanged;

        return EntityState.Modified;
    }

    public async Task<TMetadata> DeleteAsync(string postId)
    {
        return await _database.GetEntityCollection<TMetadata>()
            .FindOneAndDeleteAsync(x => x.Id == postId);
    }

    public async Task<TMetadata> SingleAsync(string postId)
    {
        return await _database.GetEntityCollection<TMetadata>()
            .Find(x => x.Id == postId)
            .Limit(1)
            .SingleOrDefaultAsync();
    }
}
