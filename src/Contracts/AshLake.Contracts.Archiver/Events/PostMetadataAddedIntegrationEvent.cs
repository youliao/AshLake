namespace AshLake.Contracts.Archiver.Events;
public record PostMetadataAddedIntegrationEvent<T>(string PostId) : IntegrationEvent where T : ISouceSite;
