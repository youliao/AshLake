namespace AshLake.Services.Archiver.Application.Commands;

public record StartDownloadingManyPostFiles(PostFileStatus Status,int Limit,string CronExpression);

public class StartDownloadingManyPostFilesHandler : IConsumer<StartDownloadingManyPostFiles>
{
    public Task Consume(ConsumeContext<StartDownloadingManyPostFiles> context)
    {
        var message = context.Message;
        var command = new CreateManyPostFileDownloadTasks(message.Status, message.Limit);

        RecurringJob.AddOrUpdate<PostFileJobs>("downloadmanypostfiles",
                                              x => x.DownloadManyPostFilesJob(command),
                                              message.CronExpression ?? "0 0/10 * * * ?");

        return Task.CompletedTask;
    }
}
