namespace AshLake.Services.Archiver.Application.Commands;

public record StopInitializingPostFileStatusCommand();

public class StopInitializingPostFileStatusCommandConsumer : IConsumer<StopInitializingPostFileStatusCommand>
{

    public Task Consume(ConsumeContext<StopInitializingPostFileStatusCommand> context)
    {
        RecurringJob.RemoveIfExists("initializepostfilestatus");

        return Task.CompletedTask;
    }
}
