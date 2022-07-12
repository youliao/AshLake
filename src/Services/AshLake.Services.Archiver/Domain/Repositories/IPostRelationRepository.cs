using System.Linq.Expressions;

namespace AshLake.Services.Archiver.Domain.Repositories;

public interface IPostRelationRepository
{
    Task AddOrUpdatePostIdAsync<T>(PostRelation postRelation) where T : IBooru;

    Task AddOrUpdatePostIdAsync<T>(IEnumerable<PostRelation> postRelations) where T : IBooru;

    Task UpdateFileStatus(PostRelation postRelation);

    Task UpdateFileStatus(IEnumerable<PostRelation> postRelations);

    Task<PostRelation> SingleAsync(string id);

    Task<IEnumerable<PostRelation>> FindAsync(Expression<Func<PostRelation, bool>> filter, int limit = 0);

    Task<long> CountAsync(Expression<Func<PostRelation, bool>> filter);
}
