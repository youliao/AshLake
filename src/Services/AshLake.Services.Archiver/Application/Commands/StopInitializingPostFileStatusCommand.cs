namespace AshLake.Services.Archiver.Application.Commands;

public record StopInitializingPostFileStatusCommand():IRequest;

public class StopInitializingPostFileStatusCommandHandler : IRequestHandler<StopInitializingPostFileStatusCommand>
{
    public Task<Unit> Handle(StopInitializingPostFileStatusCommand request, CancellationToken cancellationToken)
    {
        RecurringJob.RemoveIfExists("initializepostrelation");

        return Task.FromResult(Unit.Value);
    }
}
