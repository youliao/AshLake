namespace AshLake.Contracts.Archiver.Events;
public record PostMetadataModifiedIntegrationEvent<T>(IReadOnlyList<string> PostIds) : IntegrationEvent where T : ISouceSite;
