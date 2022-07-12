namespace AshLake.Services.Archiver.Application.Commands;

public record InitializePostFileStatusCommand(int Limit,string CronExpression);

public class InitializePostFileStatusCommandConsumer : IConsumer<InitializePostFileStatusCommand>
{

    public Task Consume(ConsumeContext<InitializePostFileStatusCommand> context)
    {
        var command = context.Message;

        RecurringJob.AddOrUpdate<PostFileJob>("initializepostfilestatus",
                                              x => x.InitializePostFileStatus(command.Limit),
                                              command.CronExpression ?? "0 0/1 * * * ?");

        return Task.CompletedTask;
    }
}
