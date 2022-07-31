namespace AshLake.Contracts.Archiver.Events;

public record KonachanPostMetadataAdded(IList<int> PostIds);

public record KonachanPostMetadataModified(IList<int> PostIds);

public record KonachanPostMetadataUnchanged(IList<int> PostIds);

public record KonachanTagMetadataChanged(int TagType);
