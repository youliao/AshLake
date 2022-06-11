namespace AshLake.Contracts.Archiver.Events;
public record TagMetadataChangedIntegrationEvent<T>(int tagType) : IntegrationEvent where T : ISouceSite;
