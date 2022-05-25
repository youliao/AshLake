namespace AshLake.Contracts.Archiver.Events;
public record PostMetadataUpdatedIntegrationEvent<T>(string PostId) : IntegrationEvent where T : ISouceSite;
