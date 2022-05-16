namespace AshLake.Services.Archiver.Application.Commands.CreateJobsForAddOrUpdateMetadata;

public abstract record CreateJobsForAddOrUpdateMetadataCommand
{
    public int StartId { get; init; }
    public int EndId { get; init; }
}

public abstract class CreateJobsForAddOrUpdateMetadataCommandHandler<TCommand>
    where TCommand : CreateJobsForAddOrUpdateMetadataCommand
{
    public async Task<IEnumerable<string>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var tasks = new List<string>();
        for (int i = command.StartId; i < command.EndId; i += 100)
        {
            tasks.Add(BackgroundJob.Enqueue<PostMetadataJobService>(x => x.AddOrUpdatePostMetadata(i, 100)));
        }
        return tasks;
    }
}
