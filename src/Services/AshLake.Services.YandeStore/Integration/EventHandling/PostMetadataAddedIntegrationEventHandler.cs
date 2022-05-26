using AshLake.BuildingBlocks.EventBus.Abstractions;
using AshLake.Contracts.Archiver.Events;
using AshLake.Services.YandeStore.Integration.ArchiverServices;
using MapsterMapper;

namespace AshLake.Services.YandeStore.Integration.EventHandling;

public class PostMetadataAddedIntegrationEventHandler
    : IIntegrationEventHandler<PostMetadataAddedIntegrationEvent<Yande>>
{

    private readonly IYandeArchiverService _archiverService;
    private readonly IMediator _mediator;

    public PostMetadataAddedIntegrationEventHandler(IYandeArchiverService archiverService, IMediator mediator)
    {
        _archiverService = archiverService;
        _mediator = mediator;
    }

    public async Task Handle(PostMetadataAddedIntegrationEvent<Yande> e)
    {
        var metadata = await _archiverService.GetPostMetadata(int.Parse(e.PostId));

        var command = metadata.Adapt<AddPostCommand>();

        await _mediator.Send(command);
    }
}