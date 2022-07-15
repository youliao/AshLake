namespace AshLake.Services.Archiver.Application.Commands;

public record CreateAddPostMetadataJobsCommand<T>(int StartId, int EndId, int Step) : IRequest 
    where T : Booru;

public class CreateAddPostMetadataJobsCommandHandler<T> : IRequestHandler<CreateAddPostMetadataJobsCommand<T>> where T : Booru
{
    public Task<Unit> Handle(CreateAddPostMetadataJobsCommand<T> command, CancellationToken cancellationToken)
    {
        var queue = typeof(T).Name.ToLower();

        for (int i = command.StartId; i <= command.EndId; i += command.Step)
        {
            int startId = i;
            int endId = i + command.Step - 1;
            endId = Math.Min(endId, command.EndId);

            BackgroundJob.Enqueue<PostMetadataJob<T>>(
                x => x.AddPostMetadata(queue, startId, endId, command.Step));
        }

        return Task.FromResult(Unit.Value);
    }
}
