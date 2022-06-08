using AshLake.Services.Compressor.Domain.Entities;

namespace AshLake.Services.Compressor.Domain.Repositories;

public interface IS3ObjectRepositoty<T> where T : IS3Object
{
    Task PutAsync(T post);

    Task<bool> ExistsAsync(string objectKey);

    Task RemoveAsync(string objectKey);
}
