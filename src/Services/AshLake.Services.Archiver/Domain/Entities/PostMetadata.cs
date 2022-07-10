using MongoDB.Bson.Serialization.Attributes;

namespace AshLake.Services.Archiver.Domain.Entities;

public record PostMetadata : Metadata
{
}

public static class PostMetadataExtensions
{
    public static string? GetMd5<T>(this PostMetadata metadata) where T : IBooru
    {
        var md5Key = typeof(T).Name switch
        {
            nameof(Yandere)=> YanderePostMetadataKeys.md5,
            nameof(Danbooru) => DanbooruPostMetadataKeys.md5,
            nameof(Konachan) => KonachanPostMetadataKeys.md5,
            _ => throw new ArgumentException(typeof(T).Name)
        };

        if(metadata.Data.TryGetValue(md5Key, out var md5BsonValue))
        {
            return md5BsonValue.AsString;
        }
        else
        {
            return null;
        }
    }

    public static string? GetExt<T>(this PostMetadata metadata) where T : IBooru
    {
        var extKey = typeof(T).Name switch
        {
            nameof(Yandere) => YanderePostMetadataKeys.file_ext,
            nameof(Danbooru) => DanbooruPostMetadataKeys.file_ext,
            nameof(Konachan) => null,
            _ => throw new ArgumentException(typeof(T).Name)
        };

        if (extKey is null & metadata.Data.TryGetValue(KonachanPostMetadataKeys.file_url, out var urlBsonValue))
        {
            return Path.GetExtension(urlBsonValue.AsString)?.TrimStart('.');
        }

        if (metadata.Data.TryGetValue(extKey, out var extBsonValue))
        {
            return extBsonValue.AsString;
        }

        return null;
    }

    public static string? GetObjectKey<T>(this PostMetadata metadata) where T : IBooru
    {
        var md5 = metadata.GetMd5<T>();
        if (md5 is null) return null;

        var ext = metadata.GetExt<T>();
        if (ext is null) return null;

        return $"{md5}.{ext}";
    }

    public static bool HasObjectKey<T>(this PostMetadata metadata) where T : IBooru
    {
        var md5 = metadata.GetMd5<T>();
        if (md5 is null) return false;

        var ext = metadata.GetExt<T>();
        if (ext is null) return false;

        return true;
    }
}