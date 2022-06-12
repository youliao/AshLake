namespace AshLake.Contracts.Collector.Events;
public record PostFileChangedIntegrationEvent(string ObjectKey) : IntegrationEvent;