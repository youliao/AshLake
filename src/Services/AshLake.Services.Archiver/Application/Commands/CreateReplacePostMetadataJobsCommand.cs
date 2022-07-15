namespace AshLake.Services.Archiver.Application.Commands;

public record CreateReplacePostMetadataJobsCommand<T>(int StartId, int EndId, int Step) where T : Booru;

public class CreateReplacePostMetadataJobsCommandConsumer<T> : IConsumer<CreateReplacePostMetadataJobsCommand<T>> where T : Booru
{
    public Task Consume(ConsumeContext<CreateReplacePostMetadataJobsCommand<T>> context)
    {
        var command = context.Message;
        var queue = typeof(T).Name.ToLower();

        for (int i = command.StartId; i <= command.EndId; i += command.Step)
        {
            int startId = i;
            int endId = i + command.Step - 1;
            endId = Math.Min(endId, command.EndId);

            BackgroundJob.Enqueue<PostMetadataJob<T>>(
                x => x.ReplacePostMetadata(queue, startId, endId, command.Step));
        }

        return Task.CompletedTask;
    }
}
