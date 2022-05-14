namespace AshLake.Services.ArchiveBox.Domain.Repositories;

public interface IMetadataRepository<T> where T : Mesadata
{
    IQueryable<T> Query(Expression<Func<T, bool>> predicate);

    Task AddAsync(T post);

    Task DeleteAsync(string postId);

    Task<T> SingleAsync(string postId);
}
