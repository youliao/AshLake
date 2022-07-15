namespace AshLake.Contracts.Archiver.Events;

public static class EventBuilders<T> where T : Booru
{
    public static dynamic PostMetadataAddedIntegrationEventBuilder(IReadOnlyList<int> PostIds)
    {
        var souceSite = typeof(T).Name;

        dynamic integrationEvent = souceSite switch
        {
            nameof(Yandere) => new YanderePostMetadataAddedEvent(PostIds),
            nameof(Danbooru) => new DanbooruPostMetadataAddedEvent(PostIds),
            nameof(Konachan) => new KonachanPostMetadataAddedEvent(PostIds),
            _ => throw new ArgumentOutOfRangeException(nameof(souceSite))
        };

        return integrationEvent;
    }

    public static dynamic PostMetadataModifiedIntegrationEventBuilder(IReadOnlyList<int> PostIds)
    {
        var souceSite = typeof(T).Name;

        dynamic integrationEvent = souceSite switch
        {
            nameof(Yandere) => new YanderePostMetadataModifiedEvent(PostIds),
            nameof(Danbooru) => new DanbooruPostMetadataModifiedEvent(PostIds),
            nameof(Konachan) => new KonachanPostMetadataModifiedEvent(PostIds),
            _ => throw new ArgumentOutOfRangeException(nameof(souceSite))
        };

        return integrationEvent;
    }

    public static dynamic PostMetadataUnchangedIntegrationEventBuilder(IReadOnlyList<int> PostIds)
    {
        var souceSite = typeof(T).Name;

        dynamic integrationEvent = souceSite switch
        {
            nameof(Yandere) => new YanderePostMetadataUnchangedEvent(PostIds),
            nameof(Danbooru) => new DanbooruPostMetadataUnchangedEvent(PostIds),
            nameof(Konachan) => new KonachanPostMetadataUnchangedEvent(PostIds),
            _ => throw new ArgumentOutOfRangeException(nameof(souceSite))
        };

        return integrationEvent;
    }
}
