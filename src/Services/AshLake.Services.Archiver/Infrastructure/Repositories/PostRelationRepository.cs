using AshLake.Services.Archiver.Infrastructure.Extensions;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public class PostRelationRepository : IPostRelationRepository
{
    private readonly IMongoDatabase _database;
    private readonly UpdateOptions _updateOptions;

    public PostRelationRepository(MongoClient mongoClient)
    {
        _database = mongoClient.GetDatabase("Common");
        CreateIndex().Wait();

        _updateOptions = new UpdateOptions { IsUpsert = true };
    }

    public async Task AddOrUpdatePostIdAsync<T>(PostRelation postRelation) where T : IBooru
    {
        var definition = typeof(T).Name switch 
        {
            nameof(Yandere) => Builders<PostRelation>.Update.Set(x => x.YandereId, postRelation.YandereId),
            nameof(Danbooru) => Builders<PostRelation>.Update.Set(x => x.DanbooruId, postRelation.DanbooruId),
            nameof(Konachan) => Builders<PostRelation>.Update.Set(x => x.KonachanId, postRelation.KonachanId),
            _ => throw new ArgumentException(typeof(T).Name)
        };

        await _database.GetEntityCollection<PostRelation>().UpdateOneAsync(x => x.Id == postRelation.Id, definition, _updateOptions);

    }

    public async Task AddOrUpdatePostIdAsync<T>(IEnumerable<PostRelation> postRelations) where T : IBooru
    {
        var bulkModels = typeof(T).Name switch
        {
            nameof(Yandere) => postRelations.Select(item => new UpdateOneModel<PostRelation>(Builders<PostRelation>.Filter.Eq(x => x.Id, item.Id),
                                                                                             Builders<PostRelation>.Update.Set(x => x.YandereId, item.YandereId)) { IsUpsert = true }),

            nameof(Danbooru) => postRelations.Select(item => new UpdateOneModel<PostRelation>(Builders<PostRelation>.Filter.Eq(x => x.Id, item.Id),
                                                                                              Builders<PostRelation>.Update.Set(x => x.DanbooruId, item.DanbooruId)) { IsUpsert = true }),

            nameof(Konachan) => postRelations.Select(item => new UpdateOneModel<PostRelation>(Builders<PostRelation>.Filter.Eq(x => x.Id, item.Id),
                                                                                              Builders<PostRelation>.Update.Set(x => x.KonachanId, item.KonachanId)) { IsUpsert = true }),
            _ => throw new ArgumentException(typeof(T).Name)
        };

        await _database.GetEntityCollection<PostRelation>().BulkWriteAsync(bulkModels);
    }

    public async Task UpdateFileStatus(PostRelation postRelation)
    {
        var definition = Builders<PostRelation>.Update.Set(x => x.FileStatus, postRelation.FileStatus);

        await _database.GetEntityCollection<PostRelation>().UpdateOneAsync(x => x.Id == postRelation.Id, definition);
    }

    public async Task UpdateFileStatus(IEnumerable<PostRelation> postRelations)
    {
        var bulkModels = postRelations.Select(item => new UpdateOneModel<PostRelation>(Builders<PostRelation>.Filter.Eq(x => x.Id, item.Id),
                                                                                       Builders<PostRelation>.Update.Set(x => x.FileStatus, item.FileStatus)));

        await _database.GetEntityCollection<PostRelation>().BulkWriteAsync(bulkModels);
    }

    public async Task<IEnumerable<PostRelation>> FindAsync(Expression<Func<PostRelation, bool>> filter, int limit = 0)
    {
        var findFluent = _database.GetEntityCollection<PostRelation>().Find(filter);

        if (limit > 0)
        {
            findFluent = findFluent.Limit(limit);
        }

        return await findFluent.ToListAsync();
    }

    public async Task<PostRelation> SingleAsync(string objectKey)
    {
        return await _database.GetEntityCollection<PostRelation>()
            .Find(x => x.Id == objectKey)
            .Limit(1)
            .SingleOrDefaultAsync();
    }

    private async Task CreateIndex()
    {
        string indexName = "FILESTATUS_INDEX";
        using (var cursor = await _database.GetEntityCollection<PostRelation>().Indexes.ListAsync())
        {
            await cursor.ForEachAsync(document =>
            {
                if (document.GetValue("name").AsString == indexName) return;
            });
        }

        var indexKeysDefinition = Builders<PostRelation>.IndexKeys.Ascending(x => x.FileStatus);

        var indexModel = new CreateIndexModel<PostRelation>(indexKeysDefinition,
                                                            new CreateIndexOptions { Background = true, Name = indexName });
        await _database.GetEntityCollection<PostRelation>().Indexes.CreateOneAsync(indexModel);
    }
}
