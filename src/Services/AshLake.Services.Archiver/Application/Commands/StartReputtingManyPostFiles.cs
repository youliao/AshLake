namespace AshLake.Services.Archiver.Application.Commands;

public record StartReputtingManyPostFiles(int Limit,string CronExpression);

public class StartReputtingManyPostFilesHandler : IConsumer<StartReputtingManyPostFiles>
{
    public Task Consume(ConsumeContext<StartReputtingManyPostFiles> context)
    {
        var message = context.Message;
        var command = new ReputManyPostFiles(message.Limit);

        RecurringJob.AddOrUpdate<PostFileJobs>("reputmanypostfiles",
                                              x => x.ReputManyPostFilesJob(command),
                                              message.CronExpression ?? "0 0/10 * * * ?");

        return Task.CompletedTask;
    }
}
