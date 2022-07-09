namespace AshLake.Contracts.Archiver.Events;

public record KonachanPostMetadataAddedEvent(IReadOnlyList<int> PostIds);

public record KonachanPostMetadataModifiedEvent(IReadOnlyList<int> PostIds);

public record KonachanPostMetadataUnchangedEvent(IReadOnlyList<int> PostIds);

public record KonachanTagMetadataChangedEvent(int TagType);
