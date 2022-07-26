namespace AshLake.Services.Archiver.Application.Commands;

public record CreateAddPostMetadataJobs<T>(int StartId, int EndId, int Step):Request<CreateAddPostMetadataJobsResult> where T : Booru;
public record CreateAddPostMetadataJobsResult(IEnumerable<string> JobIds);

public class CreateAddPostMetadataJobsHandler<T> : IConsumer<CreateAddPostMetadataJobs<T>> where T : Booru
{
    public async Task Consume(ConsumeContext<CreateAddPostMetadataJobs<T>> context)
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
                x => x.AddPostMetadata(queue, startId, endId, command.Step));

            jobIds.Add(jobId);
        }

        await context.RespondAsync(new CreateAddPostMetadataJobsResult(jobIds));
    }
}
