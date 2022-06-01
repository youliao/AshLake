using AshLake.Services.Archiver.Infrastructure;

namespace AshLake.Services.Archiver.Domain.Repositories;

public interface IMetadataRepository<TSouceSite, TMetadata> 
    where TSouceSite : ISouceSite
    where TMetadata : Metadata
{
    Task<EntityState> AddOrUpdateAsync(TMetadata metadata);

    Task<TMetadata> DeleteAsync(string postId);

    Task<TMetadata> SingleAsync(string postId);
}
