namespace AshLake.Contracts.Archiver.Events;

public record YanderePostMetadataAdded(IReadOnlyList<int> PostIds);

public record YanderePostMetadataModified(IReadOnlyList<int> PostIds);

public record YanderePostMetadataUnchanged(IReadOnlyList<int> PostIds);

public record YandereTagMetadataChanged(int TagType);
