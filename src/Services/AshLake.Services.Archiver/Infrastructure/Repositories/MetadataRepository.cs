using AshLake.Services.Archiver.Infrastructure.Extensions;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public class MetadataRepository<TSouceSite, TMetadata> : IMetadataRepository<TSouceSite, TMetadata>
    where TSouceSite : IBooru
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

    public async Task<dynamic> AddRangeAsync(IEnumerable<TMetadata> metadataList)
    {
        var ids = metadataList.Select(x => x.Id);
        var exists = await _database.GetEntityCollection<TMetadata>().Find(x => ids.Contains(x.Id)).ToListAsync() ?? new List<TMetadata>();

        var addedIds = new List<int>();
        var modifiedIds = new List<int>();
        var unchangedIds = new List<int>();

        var bulkModels = new List<WriteModel<TMetadata>>();
        foreach (var item in metadataList)
        {
            var one = exists.SingleOrDefault(x => x.Id == item.Id);
            if (one is null)
            {
                var insertOne = new InsertOneModel<TMetadata>(item);
                bulkModels.Add(insertOne);
                addedIds.Add(item.Id);
                continue;
            }

            if (one.Data == item.Data)
            {
                unchangedIds.Add(item.Id);
                continue;
            }

            var replaceOne = new ReplaceOneModel<TMetadata>(
                Builders<TMetadata>.Filter.Eq(x => x.Id, item.Id),
                item);

            bulkModels.Add(replaceOne);
            modifiedIds.Add(item.Id);
        }

        var addRangeResult = new { AddedIds = addedIds, ModifiedIds = modifiedIds, UnchangedIds = unchangedIds };

        if (bulkModels.Count == 0) return addRangeResult;

        var bulkWriteResult = await _database.GetEntityCollection<TMetadata>().BulkWriteAsync(bulkModels);

        return addRangeResult;
    }

    public async Task<dynamic> ReplaceRangeAsync(IEnumerable<TMetadata> metadataList)
    {
        if (metadataList == null || metadataList.Count() == 0) return new { AddedIds = 0, ModifiedIds = 0 };

        var ids = metadataList.Select(x => x.Id);
        var exists = await _database.GetEntityCollection<TMetadata>().Find(x => ids.Contains(x.Id)).ToListAsync() ?? new List<TMetadata>();

        var bulkModels = metadataList.Select(x => new ReplaceOneModel<TMetadata>(Builders<TMetadata>.Filter.Eq(x => x.Id, x.Id),x){ IsUpsert = true });

        var bulkWriteResult = await _database.GetEntityCollection<TMetadata>().BulkWriteAsync(bulkModels);

        var processedIds = bulkWriteResult.ProcessedRequests.Select(x => (x as ReplaceOneModel<TMetadata>)!.Replacement.Id);
        var addedIds = bulkWriteResult.Upserts.Select(x => x.Id.AsInt32);
        var modifiedIds = processedIds.Except(addedIds);

        var updateRangeResult = new { AddedIds = addedIds, ModifiedIds = modifiedIds };

        return updateRangeResult;
    }

    public async Task<TMetadata> DeleteAsync(int id)
    {
        return await _database.GetEntityCollection<TMetadata>()
            .FindOneAndDeleteAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<TMetadata>> FindAsync(Expression<Func<TMetadata,bool>> filter)
    {
        return await _database.GetEntityCollection<TMetadata>().Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<TMetadata>> FindAsync(FilterDefinition<TMetadata> filter)
    {
        return await _database.GetEntityCollection<TMetadata>().Find(filter).ToListAsync();
    }

    public async Task<TMetadata> SingleAsync(int id)
    {
        return await _database.GetEntityCollection<TMetadata>()
            .Find(x => x.Id == id)
            .Limit(1)
            .SingleOrDefaultAsync();
    }

    public async Task<TMetadata> SingleAsync(FilterDefinition<TMetadata> filter)
    {
        return await _database.GetEntityCollection<TMetadata>()
            .Find(filter)
            .Limit(1)
            .SingleOrDefaultAsync();
    }
}
