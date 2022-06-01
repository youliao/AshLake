using AshLake.BuildingBlocks.EventBus.Abstractions;
using AshLake.Services.Collector.Application.BackgroundJobs;
using Hangfire;

namespace AshLake.Services.Collector.Integration.EventHandling;

public class PostMetadataAddedIntegrationEventHandler
    : IIntegrationEventHandler<PostMetadataAddedIntegrationEvent<Yande>>
    ,IIntegrationEventHandler<PostMetadataAddedIntegrationEvent<Danbooru>>
{

    public Task Handle(PostMetadataAddedIntegrationEvent<Yande> e)
    {
        BackgroundJob.Enqueue<YandeJob>(x => x.AddFile(int.Parse(e.PostId)));
        BackgroundJob.Enqueue<YandeJob>(x => x.AddPreview(int.Parse(e.PostId)));
        return Task.CompletedTask;
    }

    public Task Handle(PostMetadataAddedIntegrationEvent<Danbooru> e)
    {
        throw new NotImplementedException();
    }
}