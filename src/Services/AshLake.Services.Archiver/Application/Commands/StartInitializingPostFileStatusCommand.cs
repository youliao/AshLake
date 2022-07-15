namespace AshLake.Services.Archiver.Application.Commands;

public record StartInitializingPostFileStatusCommand(int Limit,string CronExpression) : IRequest;

public class StartInitializingPostFileStatusCommandHandler : IRequestHandler<StartInitializingPostFileStatusCommand>
{

    public Task<Unit> Handle(StartInitializingPostFileStatusCommand command, CancellationToken cancellationToken)
    {
        RecurringJob.AddOrUpdate<PostFileJob>("initializepostfilestatus",
                                              x => x.InitializePostFileStatus(command.Limit),
                                              command.CronExpression ?? "0 0/1 * * * ?");

        return Task.FromResult(Unit.Value);
    }
}
