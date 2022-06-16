namespace AshLake.Contracts.Archiver.Events;

public record DanbooruPostMetadataAddedIntegrationEvent(IReadOnlyList<int> PostIds) : IntegrationEvent;

public record DanbooruPostMetadataModifiedIntegrationEvent(IReadOnlyList<int> PostIds) : IntegrationEvent;

public record DanbooruPostMetadataUnchangedIntegrationEvent(IReadOnlyList<int> PostIds) : IntegrationEvent;

public record DanbooruTagMetadataChangedIntegrationEvent(int TagType) : IntegrationEvent;
