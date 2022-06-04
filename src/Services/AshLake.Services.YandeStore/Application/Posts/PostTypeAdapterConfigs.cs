using AshLake.Services.YandeStore.Infrastructure.Extensions;

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
                src => src[YandePostMetadataKeys.author].AsString)
            .Map(dest => dest.CreatedAt,
                src => DateTimeOffset.FromUnixTimeSeconds(src[YandePostMetadataKeys.created_at].ToInt64()))
            .Map(dest => dest.FileExt,
                src => src[YandePostMetadataKeys.file_ext].AsString)
            .Map(dest => dest.FileSize,
                src => src[YandePostMetadataKeys.file_size].ToInt64())
            .Map(dest => dest.FileUrl,
                src => src.ContainsKey(YandePostMetadataKeys.file_url)
                    ? src[YandePostMetadataKeys.file_url].AsString
                    : null)
            .Map(dest => dest.HasChildren,
                src => src[YandePostMetadataKeys.has_children].AsBoolean)
            .Map(dest => dest.Height,
                src => src[YandePostMetadataKeys.height].AsNullableInt32)
            .Map(dest => dest.Md5,
                src => src[YandePostMetadataKeys.md5].AsString)
            .Map(dest => dest.PostId,
                src => src[YandePostMetadataKeys.id].AsNullableInt32)
            .Map(dest => dest.ParentId,
                src => src[YandePostMetadataKeys.parent_id].AsNullableInt32)
            .Map(dest => dest.Rating,
                src => PostRating.SAFE, srcCond => srcCond[YandePostMetadataKeys.rating].AsString == "s")
            .Map(dest => dest.Rating,
                src => PostRating.QUESTIONABLE, srcCond => srcCond[YandePostMetadataKeys.rating].AsString == "q")
            .Map(dest => dest.Rating,
                src => PostRating.EXPLICIT, srcCond => srcCond[YandePostMetadataKeys.rating].AsString == "e")
            .Map(dest => dest.Score,
                src => src[YandePostMetadataKeys.score].AsNullableInt32)
            .Map(dest => dest.Source,
                src => src[YandePostMetadataKeys.source].AsString)
            .Map(dest => dest.Status,
                src => src[YandePostMetadataKeys.status].AsString)
            .Map(dest => dest.Tags,
                src => src[YandePostMetadataKeys.tags].AsString.Split().ToList())
            .Map(dest => dest.UpdatedAt,
                src => DateTimeOffset.FromUnixTimeSeconds(src[YandePostMetadataKeys.updated_at].ToInt64()))
            .Map(dest => dest.Width,
                src => src[YandePostMetadataKeys.width].AsNullableInt32);
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
