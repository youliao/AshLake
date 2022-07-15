namespace AshLake.Services.Archiver.Application.Commands;

public record CreateReplacePostMetadataJobsCommand<T>(int StartId, int EndId, int Step) : IRequest where T : Booru;

public class CreateReplacePostMetadataJobsCommandHandler<T> : IRequestHandler<CreateReplacePostMetadataJobsCommand<T>> where T : Booru
{
    public Task<Unit> Handle(CreateReplacePostMetadataJobsCommand<T> command, CancellationToken cancellationToken)
    {
        var queue = typeof(T).Name.ToLower();

        for (int i = command.StartId; i <= command.EndId; i += command.Step)
        {
            int startId = i;
            int endId = i + command.Step - 1;
            endId = Math.Min(endId, command.EndId);

            BackgroundJob.Enqueue<PostMetadataJob<T>>(
                x => x.ReplacePostMetadata(queue, startId, endId, command.Step));
        }

        return Task.FromResult(Unit.Value);
    }
}
