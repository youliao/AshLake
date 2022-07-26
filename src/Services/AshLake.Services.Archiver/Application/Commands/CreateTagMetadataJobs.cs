namespace AshLake.Services.Archiver.Application.Commands;

public record CreateTagMetadataJobs<T>(IEnumerable<int> TagTypes) where T : Booru;
public record CreateTagMetadataJobsResult(IEnumerable<string> JobIds);

public class CreateTagMetadataJobsHandler<T> : IConsumer<CreateTagMetadataJobs<T>> where T : Booru
{
    public async Task Consume(ConsumeContext<CreateTagMetadataJobs<T>> context)
    {
        var command = context.Message;
        var queue = typeof(T).Name.ToLower();
        var jobIds = new List<string>();

        IEnumerable<int> tagTypes = command.TagTypes ?? new List<int>() { 0, 1, 3, 4, 5, 6 };

        foreach (var item in tagTypes)
        {
            var jobId = BackgroundJob.Enqueue<TagMetadataJob<Yandere>>(
                x => x.AddOrUpdateTagMetadata(queue, item));

            jobIds.Add(jobId);
        }

        await context.RespondAsync(new CreateTagMetadataJobsResult(jobIds));
    }
}