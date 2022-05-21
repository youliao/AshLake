namespace AshLake.Services.Archiver.Domain.Repositories;

public interface IPostObjectRepositoty
{
    Task<string> AddOrUpdateAsync(string objectKey, Stream stream);

    Task DeleteAsync(string objectKey);
}
