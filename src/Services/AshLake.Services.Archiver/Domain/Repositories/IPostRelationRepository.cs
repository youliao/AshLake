namespace AshLake.Services.Archiver.Domain.Repositories;

public interface IPostRelationRepository
{
    Task AddOrUpdateAsync<T>(string id, int postId) where T : IBooru;

    Task AddOrUpdateRangeAsync<T>(Dictionary<string, int> dic) where T : IBooru;

    Task<PostRelation> SingleAsync(string id);
}
