namespace AshLake.Services.YandeStore.Application.Posts;

public static class PostTypeAdapterConfigs
{
    public static void AddPostCommandTypeAdapterConfig()
    {
        TypeAdapterConfig<BsonDocument, AddPostCommand>
            .NewConfig()
            .Map(dest => dest.Author,
                src => src[YandePostMetadataKeys.author].AsString)
            .Map(dest => dest.CreatedAt,
                src => DateTimeOffset.FromUnixTimeSeconds(src[YandePostMetadataKeys.created_at].AsInt32))
            .Map(dest => dest.FileExt,
                src => src[YandePostMetadataKeys.file_ext].AsString)
            .Map(dest => dest.FileSize,
                src => src[YandePostMetadataKeys.file_size].ToInt64())
            .Map(dest => dest.FileUrl,
                src => src[YandePostMetadataKeys.file_url].AsString)
            .Map(dest => dest.HasChildren,
                src => src[YandePostMetadataKeys.has_children].AsBoolean)
            .Map(dest => dest.Height,
                src => src[YandePostMetadataKeys.height].AsInt32)
            .Map(dest => dest.Md5,
                src => src[YandePostMetadataKeys.md5].AsString)
            .Map(dest => dest.PostId,
                src => src[YandePostMetadataKeys.id].AsInt32)
            .Map(dest => dest.ParentId,
                src => src[YandePostMetadataKeys.parent_id].AsNullableInt32)
            .Map(dest => dest.Rating,
                src => PostRating.SAFE, srcCond => srcCond[YandePostMetadataKeys.rating].AsString == "s")
            .Map(dest => dest.Rating,
                src => PostRating.QUESTIONABLE, srcCond => srcCond[YandePostMetadataKeys.rating].AsString == "q")
            .Map(dest => dest.Rating,
                src => PostRating.EXPLICIT, srcCond => srcCond[YandePostMetadataKeys.rating].AsString == "e")
            .Map(dest => dest.Score,
                src => src[YandePostMetadataKeys.score].AsInt32)
            .Map(dest => dest.Source,
                src => src[YandePostMetadataKeys.source].AsString)
            .Map(dest => dest.Status,
                src => Enum.Parse<PostStatus>(src[YandePostMetadataKeys.status].AsString.ToUpper()))
            .Map(dest => dest.Tags,
                src => src[YandePostMetadataKeys.tags].AsString.Split().ToList())
            .Map(dest => dest.UpdatedAt,
                src => DateTimeOffset.FromUnixTimeSeconds(src[YandePostMetadataKeys.updated_at].AsInt32))
            .Map(dest => dest.Width,
                src => src[YandePostMetadataKeys.width].AsInt32);
    }
}
