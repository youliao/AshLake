namespace AshLake.Services.Archiver.Application.Commands;

public record StartInitializingPostRelationCommand(int Limit,string CronExpression);

public class StartInitializingPostRelationCommandCommandHandler : IConsumer<StartInitializingPostRelationCommand>
{
    public Task Consume(ConsumeContext<StartInitializingPostRelationCommand> context)
    {
        var command = context.Message;

        RecurringJob.AddOrUpdate<PostFileJob>("initializepostrelation",
                                              x => x.InitializePostRelation(command.Limit),
                                              command.CronExpression ?? "0 0/1 * * * ?");

        return Task.CompletedTask;
    }
}
