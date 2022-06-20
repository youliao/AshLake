namespace AshLake.Contracts.Archiver.Events;

public static class EventBuilders<T> where T : ISouceSite
{
    public static IntegrationEvent PostMetadataAddedIntegrationEventBuilder(IReadOnlyList<int> PostIds)
    {
        var souceSite = typeof(T).Name;

        IntegrationEvent integrationEvent = souceSite switch
        {
            nameof(Yande) => new YandePostMetadataAddedIntegrationEvent(PostIds),
            nameof(Danbooru) => new DanbooruPostMetadataAddedIntegrationEvent(PostIds),
            nameof(Konachan) => new KonachanPostMetadataAddedIntegrationEvent(PostIds),
            _ => throw new ArgumentOutOfRangeException(nameof(souceSite))
        };

        return integrationEvent;
    }

    public static IntegrationEvent PostMetadataModifiedIntegrationEventBuilder(IReadOnlyList<int> PostIds)
    {
        var souceSite = typeof(T).Name;

        IntegrationEvent integrationEvent = souceSite switch
        {
            nameof(Yande) => new YandePostMetadataModifiedIntegrationEvent(PostIds),
            nameof(Danbooru) => new DanbooruPostMetadataModifiedIntegrationEvent(PostIds),
            nameof(Konachan) => new KonachanPostMetadataModifiedIntegrationEvent(PostIds),
            _ => throw new ArgumentOutOfRangeException(nameof(souceSite))
        };

        return integrationEvent;
    }

    public static IntegrationEvent PostMetadataUnchangedIntegrationEventBuilder(IReadOnlyList<int> PostIds)
    {
        var souceSite = typeof(T).Name;

        IntegrationEvent integrationEvent = souceSite switch
        {
            nameof(Yande) => new YandePostMetadataUnchangedIntegrationEvent(PostIds),
            nameof(Danbooru) => new DanbooruPostMetadataUnchangedIntegrationEvent(PostIds),
            nameof(Konachan) => new KonachanPostMetadataUnchangedIntegrationEvent(PostIds),
            _ => throw new ArgumentOutOfRangeException(nameof(souceSite))
        };

        return integrationEvent;
    }
}
