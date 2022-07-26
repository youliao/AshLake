namespace AshLake.Services.Archiver.Application.Commands;

public record StartInitializingPostRelation(int Limit,string CronExpression);

public class StartInitializingPostRelationHandler : IConsumer<StartInitializingPostRelation>
{
    public Task Consume(ConsumeContext<StartInitializingPostRelation> context)
    {
        var command = context.Message;

        RecurringJob.AddOrUpdate<PostFileJob>("initializepostrelation",
                                              x => x.InitializePostRelation(command.Limit),
                                              command.CronExpression ?? "0 0/1 * * * ?");

        return Task.CompletedTask;
    }
}
