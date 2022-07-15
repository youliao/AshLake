namespace AshLake.Services.Archiver.Application.Commands;

public record StartInitializingPostRelationCommand(int Limit,string CronExpression) : IRequest;

public class StartInitializingPostRelationCommandCommandHandler : IRequestHandler<StartInitializingPostRelationCommand>
{

    public Task<Unit> Handle(StartInitializingPostRelationCommand command, CancellationToken cancellationToken)
    {
        RecurringJob.AddOrUpdate<PostFileJob>("initializepostrelation",
                                              x => x.InitializePostRelation(command.Limit),
                                              command.CronExpression ?? "0 0/1 * * * ?");

        return Task.FromResult(Unit.Value);
    }
}
