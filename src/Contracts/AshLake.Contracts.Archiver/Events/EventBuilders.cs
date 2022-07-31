namespace AshLake.Contracts.Archiver.Events;

public static class EventBuilders<T> where T : Booru
{
    public static dynamic PostMetadataAddedIntegrationEventBuilder(IList<int> PostIds)
    {
        var souceSite = typeof(T).Name;

        dynamic integrationEvent = souceSite switch
        {
            nameof(Yandere) => new YanderePostMetadataAdded(PostIds),
            nameof(Danbooru) => new DanbooruPostMetadataAdded(PostIds),
            nameof(Konachan) => new KonachanPostMetadataAdded(PostIds),
            _ => throw new ArgumentOutOfRangeException(nameof(souceSite))
        };

        return integrationEvent;
    }

    public static dynamic PostMetadataModifiedIntegrationEventBuilder(IList<int> PostIds)
    {
        var souceSite = typeof(T).Name;

        dynamic integrationEvent = souceSite switch
        {
            nameof(Yandere) => new YanderePostMetadataModified(PostIds),
            nameof(Danbooru) => new DanbooruPostMetadataModified(PostIds),
            nameof(Konachan) => new KonachanPostMetadataModified(PostIds),
            _ => throw new ArgumentOutOfRangeException(nameof(souceSite))
        };

        return integrationEvent;
    }

    public static dynamic PostMetadataUnchangedIntegrationEventBuilder(IList<int> PostIds)
    {
        var souceSite = typeof(T).Name;

        dynamic integrationEvent = souceSite switch
        {
            nameof(Yandere) => new YanderePostMetadataUnchanged(PostIds),
            nameof(Danbooru) => new DanbooruPostMetadataUnchanged(PostIds),
            nameof(Konachan) => new KonachanPostMetadataUnchanged(PostIds),
            _ => throw new ArgumentOutOfRangeException(nameof(souceSite))
        };

        return integrationEvent;
    }
}
