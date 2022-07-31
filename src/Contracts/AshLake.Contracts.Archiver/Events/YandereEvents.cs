namespace AshLake.Contracts.Archiver.Events;

public record YanderePostMetadataAdded(IList<int> PostIds);

public record YanderePostMetadataModified(IList<int> PostIds);

public record YanderePostMetadataUnchanged(IList<int> PostIds);

public record YandereTagMetadataChanged(int TagType);
