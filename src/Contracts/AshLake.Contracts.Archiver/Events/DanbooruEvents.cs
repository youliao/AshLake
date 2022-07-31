namespace AshLake.Contracts.Archiver.Events;

public record DanbooruPostMetadataAdded(IList<int> PostIds);

public record DanbooruPostMetadataModified(IList<int> PostIds);

public record DanbooruPostMetadataUnchanged(IList<int> PostIds);

public record DanbooruTagMetadataChanged(int TagType);
