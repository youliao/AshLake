using AshLake.BuildingBlocks.EventBus.Abstractions;
using AshLake.Contracts.Archiver.Events;
using AshLake.Contracts.Seedwork.SourceSites;

namespace AshLake.Services.YandeStore.Integration.EventHandling;

public class PostMetadataAddedIntegrationEventHandler
    : IIntegrationEventHandler<PostMetadataAddedIntegrationEvent<Yande>>
{
    public Task Handle(PostMetadataAddedIntegrationEvent<Yande> e)
    {
        throw new NotImplementedException();
    }
}