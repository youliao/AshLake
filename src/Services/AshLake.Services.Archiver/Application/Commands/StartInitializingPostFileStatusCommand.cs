namespace AshLake.Services.Archiver.Application.Commands;

public record StartInitializingPostFileStatusCommand(int Limit,string CronExpression);

public class StartInitializingPostFileStatusCommandConsumer : IConsumer<StartInitializingPostFileStatusCommand>
{

    public Task Consume(ConsumeContext<StartInitializingPostFileStatusCommand> context)
    {
        var command = context.Message;

        RecurringJob.AddOrUpdate<PostFileJob>("initializepostfilestatus",
                                              x => x.InitializePostFileStatus(command.Limit),
                                              command.CronExpression ?? "0 0/1 * * * ?");

        return Task.CompletedTask;
    }
}
