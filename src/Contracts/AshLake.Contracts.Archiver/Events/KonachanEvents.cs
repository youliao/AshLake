namespace AshLake.Contracts.Archiver.Events;

public record KonachanPostMetadataAddedIntegrationEvent(IReadOnlyList<int> PostIds) : IntegrationEvent;

public record KonachanPostMetadataModifiedIntegrationEvent(IReadOnlyList<int> PostIds) : IntegrationEvent;

public record KonachanPostMetadataUnchangedIntegrationEvent(IReadOnlyList<int> PostIds) : IntegrationEvent;

public record KonachanTagMetadataChangedIntegrationEvent(int TagType) : IntegrationEvent;
