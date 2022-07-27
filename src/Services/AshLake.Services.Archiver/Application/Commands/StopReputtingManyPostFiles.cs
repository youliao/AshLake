namespace AshLake.Services.Archiver.Application.Commands;

public record StopReputtingManyPostFiles();

public class StopReputtingManyPostFilesHandler : IConsumer<StopReputtingManyPostFiles>
{
    public Task Consume(ConsumeContext<StopReputtingManyPostFiles> context)
    {
        RecurringJob.RemoveIfExists("reputmanypostfiles");

        return Task.CompletedTask;
    }
}
