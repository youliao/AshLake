namespace AshLake.Services.YandeStore.Infrastructure.Extensions;

public static class BsonDocumentExtensions
{
    public static bool ContainsKey(this BsonDocument document, string key)
    {
        return document.TryGetValue(key, out _);
    }
}
