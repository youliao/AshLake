﻿namespace AshLake.Services.Archiver.Application.Commands;

public record StopDownloadingManyPostFilesCommand();

public class StopDownloadingManyPostFilesCommandConsumer : IConsumer<StopDownloadingManyPostFilesCommand>
{

    public Task Consume(ConsumeContext<StopDownloadingManyPostFilesCommand> context)
    {
        RecurringJob.RemoveIfExists("downloadmanypostfiles");
        RecurringJob.RemoveIfExists("syncpostfilestatus");

        return Task.CompletedTask;
    }
}
