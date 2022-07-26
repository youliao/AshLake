namespace AshLake.Services.Archiver.Application.Commands;

public record StopInitializingPostRelation();

public class StopInitializingPostRelationHandler : IConsumer<StopInitializingPostRelation>
{
    public Task Consume(ConsumeContext<StopInitializingPostRelation> context)
    {
        RecurringJob.RemoveIfExists("initializepostrelation");
        return Task.CompletedTask;
    }
}
