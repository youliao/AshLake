﻿namespace AshLake.Services.Archiver.Infrastructure.Services;

public interface IGrabberService<T> where T : ISouceSite
{
    Task<IEnumerable<BsonDocument>> GetPostMetadataList(int startId, int limit);

    Task<string?> GetPostObjectKey(int postId);

    Task<IEnumerable<BsonDocument>> GetTagMetadataList(int type);
}