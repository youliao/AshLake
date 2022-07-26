namespace AshLake.Contracts.Archiver.Events;

public record KonachanPostMetadataAdded(IReadOnlyList<int> PostIds);

public record KonachanPostMetadataModified(IReadOnlyList<int> PostIds);

public record KonachanPostMetadataUnchanged(IReadOnlyList<int> PostIds);

public record KonachanTagMetadataChanged(int TagType);
