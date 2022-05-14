namespace AshLake.Services.Archiver.Application.Commands.AddPostMetadata;

public record AddYandePostMetadataCommand : AddPostMetadataCommand, 
    IRequest<ArchiveStatus>;

public class AddYandePostMetadataCommandHandler : AddPostMetadataCommandHandler<AddYandePostMetadataCommand, IYandeMetadataRepository<PostMetadata>>,
    IRequestHandler<AddYandePostMetadataCommand, ArchiveStatus>
{
    public AddYandePostMetadataCommandHandler(IYandeMetadataRepository<PostMetadata> repository) : base(repository)
    {
    }
}

