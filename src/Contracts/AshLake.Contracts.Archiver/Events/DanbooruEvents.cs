namespace AshLake.Contracts.Archiver.Events;

public record DanbooruPostMetadataAddedEvent(IReadOnlyList<int> PostIds);

public record DanbooruPostMetadataModifiedEvent(IReadOnlyList<int> PostIds);

public record DanbooruPostMetadataUnchangedEvent(IReadOnlyList<int> PostIds);

public record DanbooruTagMetadataChangedEvent(int TagType);
