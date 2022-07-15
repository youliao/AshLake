namespace AshLake.Services.Archiver.Application.Commands;

public record StopDownloadingManyPostFilesCommand():IRequest;

public class StopDownloadingManyPostFilesCommandHandler : IRequestHandler<StopDownloadingManyPostFilesCommand>
{
    public Task<Unit> Handle(StopDownloadingManyPostFilesCommand command, CancellationToken cancellationToken)
    {
        RecurringJob.RemoveIfExists("downloadmanypostfiles");
        RecurringJob.RemoveIfExists("syncpostfilestatus");

        return Task.FromResult(Unit.Value);
    }
}
