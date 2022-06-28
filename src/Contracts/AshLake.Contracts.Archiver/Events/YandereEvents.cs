namespace AshLake.Contracts.Archiver.Events;

public record YanderePostMetadataAddedIntegrationEvent(IReadOnlyList<int> PostIds) : IntegrationEvent;

public record YanderePostMetadataModifiedIntegrationEvent(IReadOnlyList<int> PostIds) : IntegrationEvent;

public record YanderePostMetadataUnchangedIntegrationEvent(IReadOnlyList<int> PostIds) : IntegrationEvent;

public record YandereTagMetadataChangedIntegrationEvent(int TagType) : IntegrationEvent;
