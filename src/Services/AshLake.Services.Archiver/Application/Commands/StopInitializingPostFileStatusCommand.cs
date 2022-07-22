namespace AshLake.Services.Archiver.Application.Commands;

public record StopInitializingPostFileStatusCommand();

public class StopInitializingPostFileStatusCommandHandler : IConsumer<StopInitializingPostFileStatusCommand>
{
    public Task Consume(ConsumeContext<StopInitializingPostFileStatusCommand> context)
    {
        RecurringJob.RemoveIfExists("initializepostrelation");
        return Task.CompletedTask;
    }
}
