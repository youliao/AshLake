namespace AshLake.Services.Archiver.Domain.Repositories;

public interface IPostObjectRepositoty
{
    Task AddOrUpdateAsync(string key, string base64Data);

    Task DeleteAsync(string key);
}
