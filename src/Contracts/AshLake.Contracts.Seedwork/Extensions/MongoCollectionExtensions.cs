using MongoDB.Driver;

namespace AshLake.Contracts.Seedwork.Extensions;
public static class MongoCollectionExtensions
{
    public static IMongoCollection<T> GetEntityCollection<T>(this IMongoDatabase database)
    {
        return database.GetCollection<T>(typeof(T).Name);
    }
}
