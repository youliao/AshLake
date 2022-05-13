using MongoDB.Driver;

namespace AshLake.Contracts.Seedwork.Tools;
public static class MongoCollectionExtensions
{
    public static IMongoCollection<T> GetEntityCollection<T>(this IMongoDatabase database)
    {
        return database.GetCollection<T>(typeof(T).Name);
    }
}
