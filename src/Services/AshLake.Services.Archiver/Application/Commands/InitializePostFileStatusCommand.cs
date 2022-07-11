namespace AshLake.Services.Archiver.Application.Commands;

public record InitializePostFileStatusCommand(int Limit);

public class InitializePostFileStatusCommandConsumer : IConsumer<InitializePostFileStatusCommand>
{

    public Task Consume(ConsumeContext<InitializePostFileStatusCommand> context)
    {
        var command = context.Message;

        RecurringJob.AddOrUpdate<PostFileJob>("initializepostfilestatus", x=>x.InitializePostFileStatus(command.Limit), Cron.Minutely);

        return Task.CompletedTask;
    }
}
