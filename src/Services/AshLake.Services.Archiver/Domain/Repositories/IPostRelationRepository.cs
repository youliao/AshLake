﻿namespace AshLake.Services.Archiver.Domain.Repositories;

public interface IPostRelationRepository
{
    Task AddOrUpdateAsync<T>(PostRelation postRelation) where T : IBooru;

    Task AddOrUpdateRangeAsync<T>(IEnumerable<PostRelation> postRelations) where T : IBooru;

    Task<PostRelation> SingleAsync(string id);
}