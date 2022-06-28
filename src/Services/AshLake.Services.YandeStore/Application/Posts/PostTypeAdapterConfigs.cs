using AshLake.Services.YandeStore.Infrastructure.Extensions;
using AshLake.Services.YandeStore.Infrastructure.Repositories.Posts;

namespace AshLake.Services.YandeStore.Application.Posts;

public static class PostTypeAdapterConfigs
{
    public static void Load()
    {
        AddPostCommandConfig();
        PostMetadataDtoTypeConfig();
    }

    private static void AddPostCommandConfig()
    {
        TypeAdapterConfig<BsonDocument, AddOrUpdatePostCommand>
            .NewConfig()
            .Map(dest => dest.Author,
                src => src[YanderePostMetadataKeys.author].AsString)
            .Map(dest => dest.CreatedAt,
                src => DateTimeOffset.FromUnixTimeSeconds(src[YanderePostMetadataKeys.created_at].ToInt64()))
            .Map(dest => dest.FileExt,
                src => src[YanderePostMetadataKeys.file_ext].AsString)
            .Map(dest => dest.FileSize,
                src => src[YanderePostMetadataKeys.file_size].ToInt64())
            .Map(dest => dest.FileUrl,
                src => src.ContainsKey(YanderePostMetadataKeys.file_url)
                    ? src[YanderePostMetadataKeys.file_url].AsString
                    : null)
            .Map(dest => dest.HasChildren,
                src => src[YanderePostMetadataKeys.has_children].AsBoolean)
            .Map(dest => dest.Height,
                src => src[YanderePostMetadataKeys.height].AsNullableInt32)
            .Map(dest => dest.Md5,
                src => src[YanderePostMetadataKeys.md5].AsString)
            .Map(dest => dest.PostId,
                src => src[YanderePostMetadataKeys.id].AsNullableInt32)
            .Map(dest => dest.ParentId,
                src => src[YanderePostMetadataKeys.parent_id].AsNullableInt32)
            .Map(dest => dest.Rating,
                src => PostRating.SAFE, srcCond => srcCond[YanderePostMetadataKeys.rating].AsString == "s")
            .Map(dest => dest.Rating,
                src => PostRating.QUESTIONABLE, srcCond => srcCond[YanderePostMetadataKeys.rating].AsString == "q")
            .Map(dest => dest.Rating,
                src => PostRating.EXPLICIT, srcCond => srcCond[YanderePostMetadataKeys.rating].AsString == "e")
            .Map(dest => dest.Score,
                src => src[YanderePostMetadataKeys.score].AsNullableInt32)
            .Map(dest => dest.Source,
                src => src[YanderePostMetadataKeys.source].AsString)
            .Map(dest => dest.Status,
                src => src[YanderePostMetadataKeys.status].AsString)
            .Map(dest => dest.Tags,
                src => src[YanderePostMetadataKeys.tags].AsString.Split().ToList())
            .Map(dest => dest.UpdatedAt,
                src => DateTimeOffset.FromUnixTimeSeconds(src[YanderePostMetadataKeys.updated_at].ToInt64()))
            .Map(dest => dest.Width,
                src => src[YanderePostMetadataKeys.width].AsNullableInt32);
    }

    private static void PostMetadataDtoTypeConfig()
    {
        TypeAdapterConfig<(Post, IEnumerable<int>), PostMetadataDto>
            .NewConfig()
            .Map(dest=>dest,
                src=>src.Item1)
            .Map(dest => dest.ChildIds,
                src => src.Item2);
    }
}
