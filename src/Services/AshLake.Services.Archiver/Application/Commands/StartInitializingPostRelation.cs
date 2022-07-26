namespace AshLake.Services.Archiver.Application.Commands;

public record StartInitializingPostRelation(int Limit,string CronExpression);

public class StartInitializingPostRelationHandler : IConsumer<StartInitializingPostRelation>
{
    public Task Consume(ConsumeContext<StartInitializingPostRelation> context)
    {
        var message = context.Message;

        var command = new InitializePostRelation(message.Limit);

        RecurringJob.AddOrUpdate<PostFileJobs>("initializepostrelation",
                                              x => x.InitializePostRelationJob(command),
                                              message.CronExpression ?? "0 0/1 * * * ?");

        return Task.CompletedTask;
    }
}
