namespace AshLake.Services.Archiver.Domain.Repositories;

public interface IPostObjectRepositoty
{
    Task AddOrUpdateAsync(string objectKey, byte[] data);

    Task DeleteAsync(string objectKey);
}
