using AshLake.Services.Archiver.Infrastructure.Extensions;
using MongoDB.Driver;

namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public class PostRelationRepository : IPostRelationRepository
{
    private readonly IMongoDatabase _database;
    private readonly UpdateOptions _updateOptions;

public PostRelationRepository(MongoClient mongoClient)
    {
        _database = mongoClient.GetDatabase("Common");
        _updateOptions = new UpdateOptions { IsUpsert = true };
    }

    public async Task AddOrUpdateAsync<T>(string md5, int postId) where T : IBooru
    {
        var definition = typeof(T).Name switch 
        {
            nameof(Yandere) => Builders<PostRelation>.Update.Set(x => x.YandereId, postId),
            nameof(Danbooru) => Builders<PostRelation>.Update.Set(x => x.DanbooruId, postId),
            nameof(Konachan) => Builders<PostRelation>.Update.Set(x => x.KonachanId, postId),
            _ => throw new ArgumentException(typeof(T).Name)
        };

        await _database.GetEntityCollection<PostRelation>().UpdateOneAsync(x => x.Id == md5, definition, _updateOptions);

    }

    public async Task AddOrUpdateRangeAsync<T>(Dictionary<string,int> dic) where T : IBooru
    {
        var bulkModels = typeof(T).Name switch
        {
            nameof(Yandere) => dic.Select(item => new UpdateOneModel<PostRelation>(Builders<PostRelation>.Filter.Eq(x => x.Id, item.Key), Builders<PostRelation>.Update.Set(x => x.YandereId, item.Value)) { IsUpsert = true }),
            nameof(Danbooru) => dic.Select(item => new UpdateOneModel<PostRelation>(Builders<PostRelation>.Filter.Eq(x => x.Id, item.Key), Builders<PostRelation>.Update.Set(x => x.DanbooruId, item.Value)) { IsUpsert = true }),
            nameof(Konachan) => dic.Select(item => new UpdateOneModel<PostRelation>(Builders<PostRelation>.Filter.Eq(x => x.Id, item.Key), Builders<PostRelation>.Update.Set(x => x.KonachanId, item.Value)) { IsUpsert = true }),
            _ => throw new ArgumentException(typeof(T).Name)
        };

        await _database.GetEntityCollection<PostRelation>().BulkWriteAsync(bulkModels);
    }

    public async Task<PostRelation> SingleAsync(string id)
    {
        return await _database.GetEntityCollection<PostRelation>()
            .Find(x => x.Id == id)
            .Limit(1)
            .SingleOrDefaultAsync();
    }
}
