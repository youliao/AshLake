using MongoDB.Driver;
using System.Linq.Expressions;

namespace AshLake.Services.Archiver.Domain.Repositories;

public interface IMetadataRepository<TSouceSite, TMetadata> 
    where TSouceSite : IBooru
    where TMetadata : Metadata
{
    Task<EntityState> AddOrUpdateAsync(TMetadata metadata);

    Task<AddRangeResult> AddRangeAsync(IEnumerable<TMetadata> metadatas);
    Task<ReplaceRangeResult> ReplaceRangeAsync(IEnumerable<TMetadata> metadatas);

    Task<TMetadata> DeleteAsync(int id);

    Task<TMetadata> SingleAsync(int id);

    Task<TMetadata> SingleAsync(FilterDefinition<TMetadata> filter);

    Task<IEnumerable<TMetadata>> FindAsync(Expression<Func<TMetadata, bool>> filter);
    Task<IEnumerable<TMetadata>> FindAsync(FilterDefinition<TMetadata> filter);
}
