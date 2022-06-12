namespace AshLake.Contracts.Archiver.Events;

public record PostMetadataAddedIntegrationEvent<T>(IReadOnlyList<string> PostIds) : IntegrationEvent where T : ISouceSite;

public record PostMetadataModifiedIntegrationEvent<T>(IReadOnlyList<string> PostIds) : IntegrationEvent where T : ISouceSite;

public record PostMetadataUnchangedIntegrationEvent<T>(IReadOnlyList<string> PostIds) : IntegrationEvent where T : ISouceSite;

public record TagMetadataChangedIntegrationEvent<T>(int tagType) : IntegrationEvent where T : ISouceSite;
