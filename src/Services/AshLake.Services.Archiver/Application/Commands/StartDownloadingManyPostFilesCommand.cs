namespace AshLake.Services.Archiver.Application.Commands;

public record StartDownloadingManyPostFilesCommand(int Limit,string CronExpression):IRequest;

public class StartDownloadingManyPostFilesCommandHandler : IRequestHandler<StartDownloadingManyPostFilesCommand>
{

    public Task<Unit> Handle(StartDownloadingManyPostFilesCommand command, CancellationToken cancellationToken)
    {
        RecurringJob.AddOrUpdate<PostFileJob>("downloadmanypostfiles",
                                              x => x.DownloadManyPostFiles(command.Limit),
                                              command.CronExpression ?? "0 0/10 * * * ?");

        RecurringJob.AddOrUpdate<PostFileJob>("syncpostfilestatus",
                                      x => x.SyncPostFileStatus(command.Limit),
                                      "0 0/10 * * * ?");

        return Task.FromResult(Unit.Value);
    }
}
