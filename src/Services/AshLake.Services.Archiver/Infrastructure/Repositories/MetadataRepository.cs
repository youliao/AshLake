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

    public async Task<EntityState> AddOrUpdateAsync(TMetadata metadata)
    {
        var before = await _database.GetEntityCollection<TMetadata>()
            .FindOneAndReplaceAsync(x => x.Id == metadata.Data["id"],
                                   metadata,
                                   new() { IsUpsert = true, ReturnDocument = ReturnDocument.Before });

        if (before is null) return EntityState.Added;

        if (metadata.Equals(before)) return EntityState.Unchanged;

        return EntityState.Modified;
    }

    public Task<BulkWriteResult<TMetadata>> AddRangeAsync(IEnumerable<TMetadata> metadatas)
    {
        var bulkModels = new List<WriteModel<TMetadata>>();

        foreach (var item in metadatas)
        {
            var upsertOne = new ReplaceOneModel<TMetadata>(
                Builders<TMetadata>.Filter.Eq(x => x.Id, item.Id),
                item)
            { IsUpsert = true };

            bulkModels.Add(upsertOne);
        }

        return _database.GetEntityCollection<TMetadata>().BulkWriteAsync(bulkModels);
    }

    public async Task<TMetadata> DeleteAsync(string id)
    {
        return await _database.GetEntityCollection<TMetadata>()
            .FindOneAndDeleteAsync(x => x.Id == id);
    }

    public async Task<TMetadata> SingleAsync(string id)
    {
        return await _database.GetEntityCollection<TMetadata>()
            .Find(x => x.Id == id)
            .Limit(1)
            .SingleOrDefaultAsync();
    }
}
