namespace AshLake.Contracts.Archiver.Events;

public record DanbooruPostMetadataAdded(IReadOnlyList<int> PostIds);

public record DanbooruPostMetadataModified(IReadOnlyList<int> PostIds);

public record DanbooruPostMetadataUnchanged(IReadOnlyList<int> PostIds);

public record DanbooruTagMetadataChanged(int TagType);
