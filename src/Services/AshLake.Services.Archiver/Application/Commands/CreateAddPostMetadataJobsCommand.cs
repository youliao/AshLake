namespace AshLake.Services.Archiver.Application.Commands;

public record CreateAddPostMetadataJobsCommand<T>(int StartId, int EndId, int Step) where T : IBooru;

public class CreateAddPostMetadataJobsCommandConsumer<T> : IConsumer<CreateAddPostMetadataJobsCommand<T>> where T : IBooru
{
    public Task Consume(ConsumeContext<CreateAddPostMetadataJobsCommand<T>> context)
    {
        var command = context.Message;
        var queue = typeof(T).Name.ToLower();

        for (int i = command.StartId; i <= command.EndId; i += command.Step)
        {
            int startId = i;
            int endId = i + command.Step - 1;
            endId = Math.Min(endId, command.EndId);
            BackgroundJob.Enqueue<PostMetadataJob<T>>(
                x => x.AddPostMetadata(queue, startId, endId, command.Step));
        }

        return Task.CompletedTask;
    }
}
