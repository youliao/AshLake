using AshLake.Services.Archiver.Infrastructure;

namespace AshLake.Services.Archiver.Domain.Repositories;

public interface IMetadataRepository<T> where T : Metadata
{
    Task<ArchiveStatus> AddOrUpdateAsync(T post);

    Task<T> DeleteAsync(string postId);

    Task<T> SingleAsync(string postId);
}
