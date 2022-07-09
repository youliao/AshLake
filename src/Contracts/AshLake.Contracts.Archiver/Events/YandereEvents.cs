namespace AshLake.Contracts.Archiver.Events;

public record YanderePostMetadataAddedEvent(IReadOnlyList<int> PostIds);

public record YanderePostMetadataModifiedEvent(IReadOnlyList<int> PostIds);

public record YanderePostMetadataUnchangedEvent(IReadOnlyList<int> PostIds);

public record YandereTagMetadataChangedEvent(int TagType);
