namespace AshLake.Contracts.Archiver.Events;
public record PostMetadataAddedIntegrationEvent<T>(IReadOnlyList<string> PostIds) : IntegrationEvent where T : ISouceSite;
