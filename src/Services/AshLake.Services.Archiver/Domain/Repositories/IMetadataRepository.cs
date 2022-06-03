using MongoDB.Driver;

namespace AshLake.Services.Archiver.Domain.Repositories;

public interface IMetadataRepository<TSouceSite, TMetadata> 
    where TSouceSite : ISouceSite
    where TMetadata : Metadata
{
    Task<EntityState> AddOrUpdateAsync(TMetadata metadata);

    Task<BulkWriteResult<TMetadata>> AddRangeAsync(IEnumerable<TMetadata> metadatas);

    Task<TMetadata> DeleteAsync(string id);

    Task<TMetadata> SingleAsync(string id);
}
