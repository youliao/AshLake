using MongoDB.Driver;

namespace AshLake.Services.Archiver.Infrastructure.Extensions;
public static class MongoCollectionExtensions
{
    public static IMongoCollection<T> GetEntityCollection<T>(this IMongoDatabase database)
    {
        return database.GetCollection<T>(typeof(T).Name);
    }
}
