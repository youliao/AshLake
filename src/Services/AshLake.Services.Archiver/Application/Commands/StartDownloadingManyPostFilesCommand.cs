namespace AshLake.Services.Archiver.Application.Commands;

public record StartDownloadingManyPostFilesCommand(int Limit,string CronExpression);

public class StartDownloadingManyPostFilesCommandConsumer : IConsumer<StartDownloadingManyPostFilesCommand>
{

    public Task Consume(ConsumeContext<StartDownloadingManyPostFilesCommand> context)
    {
        var command = context.Message;

        RecurringJob.AddOrUpdate<PostFileJob>("downloadmanypostfiles",
                                              x => x.DownloadManyPostFiles(command.Limit,1000),
                                              command.CronExpression ?? "0 0/10 * * * ?");

        RecurringJob.AddOrUpdate<PostFileJob>("syncpostfilestatus",
                                      x => x.SyncPostFileStatus(command.Limit),
                                      command.CronExpression ?? "0 0/5 * * * ?");
        return Task.CompletedTask;
    }
}
