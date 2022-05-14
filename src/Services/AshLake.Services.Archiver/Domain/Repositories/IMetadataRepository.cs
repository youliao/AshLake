namespace AshLake.Services.ArchiveBox.Domain.Repositories;

public interface IMetadataRepository<T> where T : Mesadata
{
    Task<T> FindOneAndReplaceAsync(T post);

    Task<T> DeleteAsync(string postId);

    Task<T> SingleAsync(string postId);
}
