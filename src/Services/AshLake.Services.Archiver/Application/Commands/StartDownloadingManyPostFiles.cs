namespace AshLake.Services.Archiver.Application.Commands;

public record StartDownloadingManyPostFiles(int Limit,string CronExpression);

public class StartDownloadingManyPostFilesHandler : IConsumer<StartDownloadingManyPostFiles>
{
    public Task Consume(ConsumeContext<StartDownloadingManyPostFiles> context)
    {
        var command = context.Message;

        RecurringJob.AddOrUpdate<PostFileJob>("downloadmanypostfiles",
                                              x => x.DownloadManyPostFiles(command.Limit),
                                              command.CronExpression ?? "0 0/10 * * * ?");

        return Task.CompletedTask;
    }
}
