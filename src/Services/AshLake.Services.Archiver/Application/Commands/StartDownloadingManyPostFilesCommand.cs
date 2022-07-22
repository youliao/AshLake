namespace AshLake.Services.Archiver.Application.Commands;

public record StartDownloadingManyPostFilesCommand(int Limit,string CronExpression);

public class StartDownloadingManyPostFilesCommandHandler : IConsumer<StartDownloadingManyPostFilesCommand>
{
    public Task Consume(ConsumeContext<StartDownloadingManyPostFilesCommand> context)
    {
        var command = context.Message;

        RecurringJob.AddOrUpdate<PostFileJob>("downloadmanypostfiles",
                                              x => x.DownloadManyPostFiles(command.Limit),
                                              command.CronExpression ?? "0 0/10 * * * ?");

        return Task.CompletedTask;
    }
}
