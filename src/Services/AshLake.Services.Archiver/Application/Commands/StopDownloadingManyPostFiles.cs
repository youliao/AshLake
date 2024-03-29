﻿namespace AshLake.Services.Archiver.Application.Commands;

public record StopDownloadingManyPostFiles();

public class StopDownloadingManyPostFilesHandler : IConsumer<StopDownloadingManyPostFiles>
{
    public Task Consume(ConsumeContext<StopDownloadingManyPostFiles> context)
    {
        RecurringJob.RemoveIfExists("downloadmanypostfiles");

        return Task.CompletedTask;
    }
}
