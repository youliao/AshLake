namespace AshLake.Contracts.Archiver.Events;

public record YandePostMetadataAddedIntegrationEvent(IReadOnlyList<int> PostIds) : IntegrationEvent;

public record YandePostMetadataModifiedIntegrationEvent(IReadOnlyList<int> PostIds) : IntegrationEvent;

public record YandePostMetadataUnchangedIntegrationEvent(IReadOnlyList<int> PostIds) : IntegrationEvent;

public record YandeTagMetadataChangedIntegrationEvent(int TagType) : IntegrationEvent;
