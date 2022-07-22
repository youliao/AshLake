namespace AshLake.Services.Archiver.Application.Commands;

public record CreateReplacePostMetadataJobsCommand<T>(int StartId, int EndId, int Step): Request<CreateReplacePostMetadataJobsResult> where T : Booru;
public record CreateReplacePostMetadataJobsResult(IEnumerable<string> JobIds);

public class CreateReplacePostMetadataJobsCommandHandler<T> : IConsumer<CreateReplacePostMetadataJobsCommand<T>> where T : Booru
{
    public async Task Consume(ConsumeContext<CreateReplacePostMetadataJobsCommand<T>> context)
    {
        var command = context.Message;
        var queue = typeof(T).Name.ToLower();

        var jobIds = new List<string>();

        for (int i = command.StartId; i <= command.EndId; i += command.Step)
        {
            int startId = i;
            int endId = i + command.Step - 1;
            endId = Math.Min(endId, command.EndId);

            var jobId = BackgroundJob.Enqueue<PostMetadataJob<T>>(
                x => x.ReplacePostMetadata(queue, startId, endId, command.Step));
            jobIds.Add(jobId);
        }

        await context.RespondAsync(new CreateReplacePostMetadataJobsResult(jobIds));
    }
}
