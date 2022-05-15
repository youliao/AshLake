namespace AshLake.Services.Archiver.Application.Queries.GetPostMetadata;

public record GetYandePostMetadataQuery : GetPostMetadataQuery,
    IRequest<PostMetadata?>
{
}

public class GetYandePostMetadataQueryHandler : GetPostMetadataQueryHandler<GetYandePostMetadataQuery, IYandeMetadataRepository<PostMetadata>>,
    IRequestHandler<GetYandePostMetadataQuery, PostMetadata?>
{
    public GetYandePostMetadataQueryHandler(IYandeMetadataRepository<PostMetadata> repository) : base(repository)
    {
    }
}
