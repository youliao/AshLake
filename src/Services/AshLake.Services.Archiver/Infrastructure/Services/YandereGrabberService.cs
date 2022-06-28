﻿using MongoDB.Bson.Serialization;

namespace AshLake.Services.Archiver.Infrastructure.Services;

public class YandereGrabberService : IGrabberService<Yandere>
{
    private readonly HttpClient _httpClient;

    public YandereGrabberService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<IEnumerable<BsonDocument>> GetPostMetadataList(int start, int limit)
    {
        var json = await _httpClient.GetStringAsync($"/api/boorus/yandere/posts?start={start}&limit={limit}");
        var list = BsonSerializer.Deserialize<BsonArray>(json)
            .Select(x => x.AsBsonDocument);

        return list ?? new List<BsonDocument>();
    }

    public async Task<IEnumerable<BsonDocument>> GetTagMetadataList(int type)
    {
        var json = await _httpClient.GetStringAsync($"/api/boorus/yandere/tagmetadata?Type={type}&Page=1&Limit=0");
        var list = BsonSerializer.Deserialize<BsonArray>(json)
            .Select(x => x.AsBsonDocument);

        return list ?? new List<BsonDocument>();
    }
}
