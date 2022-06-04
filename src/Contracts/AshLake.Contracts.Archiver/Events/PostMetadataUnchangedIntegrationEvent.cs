namespace AshLake.Contracts.Archiver.Events;
public record PostMetadataUnchangedIntegrationEvent<T>(IReadOnlyList<string> PostIds) : IntegrationEvent where T : ISouceSite;
